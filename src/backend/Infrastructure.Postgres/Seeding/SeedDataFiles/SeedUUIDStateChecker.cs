using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Postgres.Seeding.SeedDataFiles
{
    internal class SeedUUIDStateChecker
    {
        private const string FILENAME = "SeedUUIDState.json";

        private readonly ILogger _log;
        private readonly OptionsResult _opt;
        private readonly bool _isOptionsUnderFullManualControl;

        public SeedUUIDStateChecker(ILogger log, OptionsResult options, bool isOptionsUnderFullManualControl)
        {
            _log = log;
            _opt = options;
            _isOptionsUnderFullManualControl = isOptionsUnderFullManualControl;
        }

        internal OptionsResult FreshExistenDataIfNeed()
        {
            var stateFile = GetFileName();

            SeedUUIDState? stateStored = null;

            if (File.Exists(stateFile))
            {
                var json = File.ReadAllText(stateFile);
                stateStored = JsonSerializer.Deserialize<SeedUUIDState>(json);
            }

            var isMissingState = stateStored is null;
            var isUuidChanged = stateStored is not null && stateStored.UUIDMode != CurrentConfigUUID;
            var isManualToPreset = stateStored is not null && stateStored.ManualControl == true && !_isOptionsUnderFullManualControl;
            var isCanContainInconsistentData = isUuidChanged || isManualToPreset;

            if (_opt.ExistenData == ExistenData.Fresh)
                return _opt;

            if (_isOptionsUnderFullManualControl && (isMissingState || isCanContainInconsistentData))
            {
                _log.LogWarning(
                    "Seed UUID state is missing or UUID mode changed for current DbContext while ExistenData=NotDel. " +
                    "Manual mode: existing data consistency cannot be validated.");

                return _opt; // ничего не меняем только LogWarning
            }

            if (!_isOptionsUnderFullManualControl && _opt.ExistenData != ExistenData.Fresh)
            {
                if (isCanContainInconsistentData)
                {
                    // риск "InconsistentData" - UUID реально поменялся / был ручной сид раньше
                    _log.LogWarning(
                        "Detected UUID mode change or previous manual-control seeding for current DbContext. " +
                        "ExistenData is enforced to Fresh once to rebuild compatible data.");

                    return _opt with { ExistenData = ExistenData.Fresh };
                }

                if (isMissingState)
                {
                    // Нет state-файла → считаем, что это первый запуск пресета либо возможность что раньше было InconsistentData
                    _log.LogInformation(
                        "Seed UUID state not found for current DbContext, unable to verify previous state. " +
                        "ExistenData is enforced to Fresh once to initialize a consistent seed state.");

                    return _opt with { ExistenData = ExistenData.Fresh };
                }
            }

            return _opt;
        }

        // после успешного сидирования — обновляем SeedUUIDUsedState.json
        internal void StoreCurrentUsedSeedUUID()
        {
            var newState = new SeedUUIDState
            {
                UUIDMode = CurrentConfigUUID,
                ManualControl = _isOptionsUnderFullManualControl,
                LastSeedUtc = DateTime.UtcNow
            };

            File.WriteAllText(GetFileName(),
                JsonSerializer.Serialize(newState, new JsonSerializerOptions { WriteIndented = true }));
        }

        private UUIDMode CurrentConfigUUID => _opt.UUIDmode;  // "Real" / "Stable"

        private string GetFileName() => Path.Combine(_opt.PathBase, FILENAME);
    }
}

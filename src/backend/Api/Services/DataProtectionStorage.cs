using Microsoft.AspNetCore.DataProtection;

namespace Api.Services;

public static class DataProtectionStorage
{
    public static void AddStorageForContainers(this IServiceCollection services, IConfiguration cfg)
    {
        // Задает общее имя приложения (ApplicationName) —
        // нужно, если несколько контейнеров/инстансов должны использовать одни и те же ключи шифрования.
        var dp = services.AddDataProtection()
            .SetApplicationName(cfg["ASPNETCORE_DataProtection__ApplicationName"] ?? "WorkshopCode");

        var dpPath = cfg["DataProtection:Path"];

        // Если указан путь (в контейнере или launchSettings),
        // ключи будут сохраняться на диск вместо временного хранилища по умолчанию.
        if (!string.IsNullOrWhiteSpace(dpPath))
        {
            dp.PersistKeysToFileSystem(new DirectoryInfo(dpPath));
        }
    }
}

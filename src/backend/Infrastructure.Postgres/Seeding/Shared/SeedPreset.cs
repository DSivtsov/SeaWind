namespace Infrastructure.Postgres.Seeding.Shared;

/// <summary>
/// <para>SeedOFF     - сидирование полностью отключено</para>
/// <para>SeedCustom  - ручное управление всеми опциями</para>
/// <para>SuperFast   - UUID=STABLE, ExistenData=NotDel, InsertMode=InsertOnly, AutoMigrate=true</para>
/// <para>Fast        - UUID=STABLE, ExistenData=NotDel, InsertMode=InsertOrUpdate, AutoMigrate=true</para>
/// <para>Real        - UUID=Real, ExistenData=Fresh, InsertMode=InsertOnly, AutoMigrate=true</para>
/// </summary>
public enum SeedPreset
{
    SeedOFF,
    SeedCustom,
    SuperFast,
    Fast,
    Real
}

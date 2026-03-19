namespace Api.Configuration;

public static class AppsettingsOptionsDI
{
    public static IServiceCollection AddAppSettingsOptions(this IServiceCollection services, IConfiguration cfg)
    {
        services.Configure<StorageOptions>(cfg.GetSection("Storage"));

        return services;
    }
}

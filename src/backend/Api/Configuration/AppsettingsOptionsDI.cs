namespace Api.Configuration;

public static class AppsettingsOptionsDI
{
    public static IServiceCollection AddAppSettingsOptions(this IServiceCollection services, IConfiguration cfg)
    {
        // Storage configuration.
        // Contains application file storage paths (uploads, attachments, etc.).
        services.Configure<StorageOptions>(cfg.GetSection("Storage"));

        return services;
    }
}

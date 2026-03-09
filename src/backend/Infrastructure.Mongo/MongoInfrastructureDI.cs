using Infrastructure.Mongo.ExerciseChat;
using Infrastructure.Mongo.SupportChat;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure.Mongo;

public static class InfrastructureMongoDI
{
    public static IServiceCollection AddMongoInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        if (Environment.GetEnvironmentVariable("WC_USE_TEST_SETTINGS") != "true")
        {
            var connectionString = cfg.GetSection("Mongo")["ConnectionString"]
                ?? throw new InvalidOperationException("Mongo:ConnectionString is missing.");

            var dbNameExerciseChat = cfg.GetSection("Mongo")["SupportChatDatabase"]
                ?? throw new InvalidOperationException("Mongo:SupportChatDatabase is missing.");

            var dbNameSupportChat = cfg.GetSection("Mongo")["ExerciseChatDatabase"]
                ?? throw new InvalidOperationException("Mongo:ExerciseChatDatabase is missing.");

            services.AddSingleton(sp =>
            {
                connectionString = cfg["Mongo:ConnectionString"]!;
                return new MongoClient(connectionString);
            });

            services.AddSingleton<ISupportChatDb>(sp =>
            {
                var client = sp.GetRequiredService<MongoClient>();
                return new SupportChatDb(client, dbNameExerciseChat);
            });

            services.AddSingleton<IExerciseChatDb>(sp =>
            {
                var client = sp.GetRequiredService<MongoClient>();
                return new ExerciseChatDb(client, dbNameSupportChat);
            });
        }

        services.AddSupportChatRepositories();

        services.AddExerciseChatRepositories();

        return services;
    }
}

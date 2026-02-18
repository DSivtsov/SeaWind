using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Mongo;

public static class MongoInfrastructureDI
{
    public static IServiceCollection AddMongoInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        var csExerciseChat = cfg.GetSection("Mongo")["ExerciseChat"]
            ?? throw new InvalidOperationException("Mongo:ExerciseChat is missing.");

        var csSupportChat = cfg.GetSection("Mongo")["SupportChat"]
            ?? throw new InvalidOperationException("Mongo:SupportChat is missing.");

        services.AddExerciseChat(csExerciseChat);

        services.AddSupportChat(csSupportChat);

        return services;
    }
}

using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Infrastructure.Mongo;

public static class AddExerciseChatMongo
{
    internal static IServiceCollection AddExerciseChat(this IServiceCollection services, string connectionString)
    {
        Debug.WriteLine($"[AddExerciseChat]: connectionString[{connectionString}]");

        return services;
    }
}

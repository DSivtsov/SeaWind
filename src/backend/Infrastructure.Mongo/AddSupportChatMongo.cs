using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Infrastructure.Mongo;

public static class AddSupportChatMongo
{
    internal static IServiceCollection AddSupportChat(this IServiceCollection services, string connectionString)
    {
        Debug.WriteLine($"[AddSupportChat]: connectionString[{connectionString}]");

        return services;
    }
}

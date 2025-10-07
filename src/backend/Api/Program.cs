using Application;
using Infrastructure;

namespace Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //нужно добавить перед добавлением сервисов
        builder.AddConfiguration();

        builder.Services
            .AddApplication()
            .AddInfrastructure(builder.Configuration)
            .AddPresentation(builder.Configuration, builder.Environment);

        var app = builder.Build();

        app.UsePresentation();

        app.Run();
    }
}

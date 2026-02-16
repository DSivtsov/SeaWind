using Api.Filters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.StaticFiles;
using System.Text.Json.Serialization;

namespace Api;

public static class PresentationDI
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration cfg,
        IWebHostEnvironment env)
    {
        var isIntegrationTestRun = Environment.GetEnvironmentVariable("WC_USE_TEST_SETTINGS") == "true";

        // Регистрация контроллеров
        // - подключение фильтров
        // - настройка поведения сериализации JSON-ответов
        services.AddControllers(optControllers =>
            {
                // Подключаем фильтр исключений
                optControllers.Filters.Add<CustomExceptionFilter>();
            })
            .AddJsonOptions(opt =>
            {
                // Игнорировать циклические ссылки
                opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                // Не добавлять свойства со значениями null
                opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;


            })
            .AddTraceIdToProblemDetails();

        // CORS только для DEV (Vite dev-server на 5173 только по http)
        if (env.IsDevelopment() && !isIntegrationTestRun)
        {
            services.AddCors(o => o.AddPolicy("Dev", p => p
                .WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()));
            // .AllowCredentials() // если используешь cookies/SignalR в dev
        }

        // Максимальный размер загружаемых файлов = 35 Мб.
        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = 35 * 1024 * 1024;
        });

        services.AddHealthChecks();

        return services;
    }

    public static WebApplication UsePresentation(this WebApplication app)
    {
        var isIntegrationTestRun = Environment.GetEnvironmentVariable("WC_USE_TEST_SETTINGS") == "true";

        app.MapHealthChecks("/health");

        // DEV: без HTTPS, чтобы не конфликтовать с Vite
        // PROD: TLS на прокси, Kestrel — только HTTP внутри
        if (!app.Environment.IsDevelopment() && !isIntegrationTestRun)
        {
            // Не редиректим на HTTPS — это делает прокси
            // app.UseHttpsRedirection();

            // Доп. защита от downgrade-атак (работает, когда внешний трафик уже по HTTPS)
            app.UseHsts();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment() && !isIntegrationTestRun)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("Dev"); // разрешаем фронту с 5173
        }

        // В PROD SPA уже собран и лежит в wwwroot/
        // В DEV SPA раздаёт Vite: но статику для wwwroot/exercises/ раздает всегда API Server
        // но fallback здесь не включаем
        var contentTypeProvider = new FileExtensionContentTypeProvider();
        contentTypeProvider.Mappings[".cs"] = "text/plain; charset=utf-8";
        contentTypeProvider.Mappings[".mmd"] = "text/plain; charset=utf-8";
        contentTypeProvider.Mappings[".md"] = "text/markdown; charset=utf-8";

        app.UseStaticFiles(new StaticFileOptions
        {
            ContentTypeProvider = contentTypeProvider
        });

        app.UseRouting();

        // ASP.NET Identity сервисы - между UseRouting() и MapControllers()
        // и первым должен идти UseAuthentication()
        app.UseAuthentication();
        app.UseAuthorization();

        // В соответствие с REST выставить по умолчанию дял всех /api* cache "no-store"
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.OnStarting(() =>
                {
                    // Если заголовки уже выставлены (public/private/no-store) — не трогаем
                    if (context.Response.Headers.ContainsKey("Cache-Control"))
                        return Task.CompletedTask;

                    // Консервативный дефолт для API
                    context.Response.Headers["Cache-Control"] = "no-store";

                    return Task.CompletedTask;
                });
            }

            await next();
        });

        // СНАЧАЛА API-маршруты (чтобы их не перехватывал SPA-fallback)
        app.MapControllers();

        // Потом SPA-fallback (только в PROD)
        if (!app.Environment.IsDevelopment() && !isIntegrationTestRun)
        {
            app.MapFallbackToFile("{*path}", "index.html");
        }

        return app;
    }
}

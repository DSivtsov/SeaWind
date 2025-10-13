using Backend.Filters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Text.Json.Serialization;

namespace Api;

public static class PresentationDI
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration cfg,
        IWebHostEnvironment env)
    {
        // AddSwaggerGen() делается через AddSwaggerGen(this IServiceCollection services)
        // services.AddSwaggerGen();

        // Регистрация контроллеров и настройка поведения сериализации JSON-ответов
        services.AddControllers()
            .AddJsonOptions(opt =>
            {
                // Игнорировать циклические ссылки
                opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                // Не добавлять свойства со значениями null
                opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        // CORS только для DEV (Vite dev-server на 5173 только по http)
        if (env.IsDevelopment())
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

        // Подключаем фильтр исключений
        services.AddMvc(options =>
        {
            options.Filters.Add<CustomExceptionFilter>();
        });

        services.AddHealthChecks();

        return services;
    }

    public static WebApplication UsePresentation(this WebApplication app)
    {
        app.MapHealthChecks("/health");

        // DEV: без HTTPS, чтобы не конфликтовать с Vite
        // PROD: TLS на прокси, Kestrel — только HTTP внутри
        if (!app.Environment.IsDevelopment())
        {
            // Не редиректим на HTTPS — это делает прокси
            // app.UseHttpsRedirection();

            // Доп. защита от downgrade-атак (работает, когда внешний трафик уже по HTTPS)
            app.UseHsts();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("Dev"); // разрешаем фронту с 5173
            // В DEV SPA раздаёт Vite: статику и fallback здесь не включаем
        }
        else
        {
            // В PROD SPA уже собран и лежит в wwwroot/
            app.UseStaticFiles();
        }

        app.UseRouting();

        // ASP.NET Identity сервисы - между UseRouting() и MapControllers()
        // и первым должен идти UseAuthentication()
        app.UseAuthentication();
        app.UseAuthorization();

        // СНАЧАЛА API-маршруты (чтобы их не перехватывал SPA-fallback)
        app.MapControllers();

        // Потом SPA-fallback (только в PROD)
        if (!app.Environment.IsDevelopment())
        {
            app.MapFallbackToFile("{*path}", "index.html");
        }

        return app;
    }
}
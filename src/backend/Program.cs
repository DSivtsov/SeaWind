using Backend.Filters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;

namespace Backend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Настройка генерирования JSON в ответах
        builder.Services.AddControllers()
            .AddJsonOptions(opt =>
            {
                // Игнорировать циклические ссылки
                opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                // Не добавлять свойства со значениями null
                opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        // CORS только для DEV (Vite dev-server на 5173 только по http)
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddCors(o => o.AddPolicy("Dev", p => p
                .WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()));
                // .AllowCredentials() // если используешь cookies/SignalR в dev
        }

        // Максимальный размер загружаемых файлов = 35 Мб.
        builder.Services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = 35 * 1024 * 1024;
        });

        // Подключаем фильтр исключений
        builder.Services.AddMvc(options =>
        {
            options.Filters.Add<CustomExceptionFilter>();
        });

        var app = builder.Build();

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
        // Отключены пока нет ASP.NET Identity / JWT 
        //app.UseAuthentication();
        //app.UseAuthorization();

        // СНАЧАЛА API-маршруты (чтобы их не перехватывал SPA-fallback)
        app.MapControllers();

        // Потом SPA-fallback (только в PROD)
        if (!app.Environment.IsDevelopment())
        {
            app.MapFallbackToFile("{*path}", "index.html");
        }

        app.Run();
    }
}

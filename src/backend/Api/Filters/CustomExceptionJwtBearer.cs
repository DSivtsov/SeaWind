using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Net.Http.Headers;

namespace Api.Filters;

internal static class CustomExceptionJwtBearer
{
    // Настройка централизованного формата ProblemDetails (RFC 7807)
    // для всех ошибок аутентификации и авторизации.
    //
    // Также удаляем заголовок WWW-Authenticate,
    // поскольку некоторые браузеры и HTTP-клиенты реагируют на него нестандартно:
    // могут вызывать неожиданные редиректы, всплывающие окна логина
    // или возвращать невалидный JSON.
    internal static void TuneCustomJwtBearerException(this JwtBearerOptions opt)
    {
        opt.Events = new JwtBearerEvents
        {
            OnChallenge = ctx =>
            {
                ctx.HandleResponse(); // отключаем дефолтный пустой 401

                var factory = ctx.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                var problem = factory.CreateProblemDetails(
                    ctx.HttpContext,
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Unauthorized",
                    detail: "A valid Bearer token is required.",
                    instance: ctx.HttpContext.Request.Path);

                ctx.Response.StatusCode = problem.Status ?? StatusCodes.Status401Unauthorized;
                ctx.Response.ContentType = "application/problem+json";
                // Убираем заголовок WWW-Authenticate
                ctx.Response.Headers.Remove(HeaderNames.WWWAuthenticate);

                return ctx.Response.WriteAsJsonAsync(problem);
            },
            OnForbidden = ctx =>
            {
                var factory = ctx.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                var problem = factory.CreateProblemDetails(
                    ctx.HttpContext,
                    statusCode: StatusCodes.Status403Forbidden,
                    title: "Forbidden",
                    detail: "You do not have access to this resource.",
                    instance: ctx.HttpContext.Request.Path);

                ctx.Response.StatusCode = problem.Status ?? StatusCodes.Status403Forbidden;
                ctx.Response.ContentType = "application/problem+json";
                // Убираем заголовок WWW-Authenticate
                ctx.Response.Headers.Remove(HeaderNames.WWWAuthenticate);

                return ctx.Response.WriteAsJsonAsync(problem);
            }
        };
    }
}

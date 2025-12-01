namespace Api.Filters;

internal static class CustomExceptionProblemDetails
{
    internal static IServiceCollection AddCustomException(this IServiceCollection services)
    {
        // Настройка централизованного формата ProblemDetails (RFC 7807) для всех ошибок:
        // добавляет traceId (HttpContext.TraceIdentifier) в Extensions.
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = ctx =>
            {
                ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext?.TraceIdentifier;
            };
        });
        return services;
    }
}

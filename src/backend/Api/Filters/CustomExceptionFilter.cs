using Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;

namespace Api.Filters;

public class CustomExceptionFilter : IExceptionFilter
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;
    private readonly IHostEnvironment _env;

    public CustomExceptionFilter(ProblemDetailsFactory pdf, IHostEnvironment env)
    {
        _problemDetailsFactory = pdf;
        _env = env;
    }

    // Централизованное формирование ProblemDetails (RFC 7807)
    // для необработанных исключений в контроллерах.
    public void OnException(ExceptionContext context)
    {
        var ex = context.Exception;

        var status = ExceptionToStatusCodeMap.StatusCodeValues.TryGetValue(ex.GetType(), out int s)
            ? s : StatusCodes.Status500InternalServerError;

        var problem = _problemDetailsFactory.CreateProblemDetails(
            context.HttpContext,
            statusCode: status,
            title: ReasonPhrases.GetReasonPhrase(status),
            detail: GetSafeMessage(ex, status),
            instance: context.HttpContext.Request.Path
        );

        if (status == StatusCodes.Status500InternalServerError && _env.IsDevelopment())
        {
            ShowStack(ex, problem);
        }

        context.Result = new ObjectResult(problem) { StatusCode = status };

        // Пометим, что ошибка обработана
        context.ExceptionHandled = true;
    }

    private string? GetSafeMessage(Exception ex, int status)
    {
        // Клиентские ошибки (4xx) можно показывать пользователю
        if (status >= 400 && status < 500)
            return ex.Message;

        // Серверные ошибки — показываем безопасно
        if (_env.IsDevelopment())
            return $"{ex.GetType().Name}: {ex.Message}";

        return status switch
        {
            StatusCodes.Status500InternalServerError => "Internal server error.",
            StatusCodes.Status502BadGateway => "Bad gateway.",
            StatusCodes.Status503ServiceUnavailable => "Service temporarily unavailable.",
            StatusCodes.Status504GatewayTimeout => "Gateway timeout.",
            _ => "Unexpected server error."
        };
    }

    private void ShowStack(Exception ex, ProblemDetails problem)
    {
        var lines = (ex.StackTrace ?? "")
            .Replace("\r\n", "\n")
            .Split('\n', StringSplitOptions.RemoveEmptyEntries);

        problem.Extensions["debug"] = new
        {
            exception = ex.GetType().Name,
            stack = lines
        };
    }
}

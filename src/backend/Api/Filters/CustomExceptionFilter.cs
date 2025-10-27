using Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

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
            title: ex.GetType().Name,
            detail: GetSafeMessage(ex, status),
            instance: context.HttpContext.Request.Path
        );

        context.Result = new ObjectResult(problem) { StatusCode = status };

        // Пометим, что ошибка обработана
        context.ExceptionHandled = true;
    }

    private string? GetSafeMessage(Exception ex, int status)
    {
        if (status == StatusCodes.Status500InternalServerError)
        {
            // В DEV показываем полное сообщение (и stacktrace)
            if (_env.IsDevelopment())
                return $"{ex.Message}\n{ex.StackTrace}";

            // В PRD — короткое, безопасное
            return "An unexpected error occurred.";
        }

        // Для ожидаемых исключений — просто ex.Message
        return ex.Message;
    }
}

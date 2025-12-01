using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Api.Filters;

internal static class CustomExceptionInvalidModelState
{
    // Настройка централизованного формата ProblemDetails (RFC 7807)
    // для всех ошибок валидации ModelState (InvalidModelStateResponseFactory).
    internal static IMvcBuilder AddTraceIdToProblemDetails(this IMvcBuilder builder)
    {
        builder.ConfigureApiBehaviorOptions(opt =>
        {
            opt.InvalidModelStateResponseFactory = ctx =>
            {
                var factory = ctx.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();

                var problem = factory.CreateValidationProblemDetails(
                    ctx.HttpContext,
                    modelStateDictionary: ctx.ModelState,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Validation failed",
                    detail: "Your request contains invalid data.",
                    instance: ctx.HttpContext.Request.Path);

                return new BadRequestObjectResult(problem);
            };
        });

        return builder;
    }
}

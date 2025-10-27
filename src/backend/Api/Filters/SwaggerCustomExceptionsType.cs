using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Filters;

// Swagger-mapping для корректного отображения ProblemDetails и ValidationProblemDetails,
// используемых в централизованном формате ошибок (RFC 7807).
internal static class SwaggerCustomExceptionsType
{
    internal static void MapTypeProblemDetails(this SwaggerGenOptions swgOpt)
    {
        swgOpt.MapType<ProblemDetails>(() => new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["type"] = new() { Type = "string" },
                ["title"] = new() { Type = "string" },
                ["status"] = new() { Type = "integer", Format = "int32" },
                ["detail"] = new() { Type = "string" },
                ["instance"] = new() { Type = "string" },
                ["traceId"] = new() { Type = "string" }
            },
            AdditionalPropertiesAllowed = false
        });

    }

    internal static void MapTypeValidationProblemDetails(this SwaggerGenOptions swgOpt)
    {
        swgOpt.MapType<ValidationProblemDetails>(() => new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["type"] = new() { Type = "string" },
                ["title"] = new() { Type = "string" },
                ["status"] = new() { Type = "integer", Format = "int32" },
                ["detail"] = new() { Type = "string" },
                ["instance"] = new() { Type = "string" },
                ["traceId"] = new() { Type = "string" },
                ["errors"] = new()
                {
                    Type = "object",
                    AdditionalProperties = new OpenApiSchema {Type = "array", Items = new OpenApiSchema { Type = "string" } }
                }
            },
            AdditionalPropertiesAllowed = false
        });

    }
}

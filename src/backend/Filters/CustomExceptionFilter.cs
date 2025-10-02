using Backend.Exceptions;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Text.Json;

namespace Backend.Filters;

public class CustomExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        int statusCode;

        switch(context.Exception)
        {
            case BadRequestException:
                statusCode = (int)HttpStatusCode.BadRequest; break;

            case UnauthorizedException:
                statusCode = (int)HttpStatusCode.Unauthorized; break;

            case NotFoundException:
                statusCode = (int)HttpStatusCode.NotFound; break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError; break;
        }

        // Сериализуем ответ в JSON
        var json = JsonSerializer.Serialize(new ResponseDtoBase { ErrorMessage = context.Exception.Message });

        // Вернём ошибку в формате JSON
        context.Result = new ContentResult 
        { 
            Content = json,
            ContentType = "application/json",
            StatusCode = statusCode
        };

        // Пометим, что ошибка обработана
        context.ExceptionHandled = true;
    }
}

using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ApiTemplate.Application.Common.Extensions;
using ApiTemplate.Application.Common.Interfaces;
using ApiTemplate.Domain.Common.Exceptions;
using ApiTemplate.Infrastructure.Common;

namespace ApiTemplate.Infrastructure.Exceptions;

internal class ExceptionMiddleware : IMiddleware
{
    private readonly ISerializerService _jsonSerializer;
    private readonly ExceptionsSettings _exceptionsSettings;

    public ExceptionMiddleware(
        ISerializerService jsonSerializer,
        IOptions<ExceptionsSettings> exceptionsSettings
    )
    {
        _jsonSerializer = jsonSerializer;
        _exceptionsSettings = exceptionsSettings.Value;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            HttpStatusCode statusCode;
            var errorCode = "";
            object? data = null;

            switch (exception)
            {
                case CustomException e:
                    statusCode = e.StatusCode;
                    errorCode = e.Code.ToUnderScoreCase();
                    data = e.CustomData;
                    break;

                case ValidationException:
                    statusCode = HttpStatusCode.BadRequest;
                    errorCode = "validation_error";
                    break;

                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    data = exception.Data;
                    break;
            }

            var errorResult = new ErrorResult()
            {
                Code = errorCode,
                StatusCode = (int)statusCode,
                Data = data,
            };

            var currentException = exception;
            ExceptionError currentExceptionError = errorResult;

            while (true)
            {
                currentExceptionError.Message = currentException.Message.Trim();
                currentExceptionError.Source = currentException.TargetSite?.DeclaringType?.FullName!;

                HandleStackTrace(currentException, currentExceptionError);

                currentException = currentException.InnerException;

                if (currentException == null)
                    break;

                currentExceptionError.InnerError = new ExceptionError();
                currentExceptionError = currentExceptionError.InnerError;
            }

            var response = context.Response;

            if (!response.HasStarted)
            {
                response.ContentType = "application/json";
                response.StatusCode = errorResult.StatusCode;

                await response.WriteAsync(_jsonSerializer.Serialize(errorResult));
            }
        }
    }

    private void HandleStackTrace(Exception exception, ExceptionError exceptionError)
    {
        if (!_exceptionsSettings.AddStackTrace || string.IsNullOrEmpty(exception.StackTrace))
            return;

        var stackTrace = exception.StackTrace.Split("\r\n   at ");

        stackTrace[0] = stackTrace[0].Replace("   at ", "");

        exceptionError.StackTrace = stackTrace;
    }
}

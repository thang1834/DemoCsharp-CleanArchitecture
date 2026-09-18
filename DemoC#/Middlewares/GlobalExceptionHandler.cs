using DemoC_.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DemoC_.Middlewares
{
    /// <summary>
    /// Nơi duy nhất xử lý BẤT KỲ lỗi nào xảy ra trong toàn bộ hệ thống.
    /// Kế thừa giao diện IExceptionHandler của .NET 8+, giúp thay thế hoàn toàn các khối `try...catch` cồng kềnh ở Controller.
    /// Đảm bảo Client luôn nhận được JSON báo lỗi chuẩn mực theo định dạng ProblemDetails (RFC 7807).
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

            var problemDetails = new ProblemDetails
            {
                Instance = httpContext.Request.Path
            };

            switch (exception)
            {
                case ArgumentException argumentException:
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "Validation Error";
                    problemDetails.Detail = argumentException.Message;
                    break;

                case NotFoundException notFoundException:
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Title = "Resource Not Found";
                    problemDetails.Detail = notFoundException.Message;
                    break;

                default:
                    problemDetails.Status = StatusCodes.Status500InternalServerError;
                    problemDetails.Title = "Internal Server Error";
                    problemDetails.Detail = "An unexpected error occurred. Please try again later.";
                    break;
            }

            httpContext.Response.StatusCode = problemDetails.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}

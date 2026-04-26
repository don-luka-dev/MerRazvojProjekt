using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MerRazvojProjekt.Server.Exceptions
{
    internal sealed class ExceptionMiddlware(ILogger<ExceptionMiddlware> logger, IProblemDetailsService problemDetailsService) : IExceptionHandler
    {
        private readonly ILogger<ExceptionMiddlware> _logger = logger;
        private readonly IProblemDetailsService _problemDetailsService = problemDetailsService;


        //cancelaton token nije u funkciji
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Unhandled exception occured");

            var statusCode = exception switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                ArgumentException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                InvalidOperationException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            httpContext.Response.StatusCode = statusCode;

            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            { 
                HttpContext = httpContext, 
                Exception = exception ,
                ProblemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Type = exception.GetType().Name,
                    Title = statusCode switch
                    {
                        StatusCodes.Status400BadRequest => "Bad request",
                        StatusCodes.Status404NotFound => "Resource not found",
                        StatusCodes.Status409Conflict => "Conflict",
                        _ => "Server error"
                    },
                    Detail = exception.Message
                }
            });
        }
    }
}

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ExamMachineIndustryInvestmentFund.WebApi.ExceptionHandling
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
            => _logger = logger;


        public async ValueTask<bool> TryHandleAsync
        (
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken
        )
        {

            var (statusCode, title) = exception switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
                UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Forbidden"),
                ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request"),
                InvalidOperationException => (StatusCodes.Status409Conflict, "Conflict"),
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
            };


            if (statusCode.Equals(500)) 
                _logger.LogError(exception, "Unhandled exception");
            else
                _logger.LogWarning(exception, "Request Failed");


            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = statusCode.Equals(500)
                    ? "An unexpected error occured"
                    : exception.Message,
                Instance = context.Request.Path
            };


            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsJsonAsync(problem, cancellationToken);

            return true;
        }
    }
}

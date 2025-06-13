using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Medicines.Core.Exceptions;
using Serilog;
using System.Net;

namespace Medicines.API.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;

        public GlobalExceptionHandler(IProblemDetailsService problemDetailsService)
        {
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            Log.Error(exception, "An unhandled exception occurred: {Message}", exception.Message);

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "An unexpected error occured.",
                Detail = exception.Message,
                Type = exception.GetType().Name
            };

            if (exception is ResourceNotFoundException notFoundEx)
            {
                problemDetails.Status = (int)HttpStatusCode.NotFound;
                problemDetails.Title = "Resource Not Found";
                problemDetails.Detail = notFoundEx.Message;
                problemDetails.Type = "https://example.com/probs/resource-not-found";
            }
            else if (exception is ArgumentException argEx)
            {
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Invalid argument.";
                problemDetails.Detail = argEx.Message;
                problemDetails.Type = exception.GetType().Name;
            }

            var result = await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
                Exception = exception
            });

            return result;
        }
    }
}

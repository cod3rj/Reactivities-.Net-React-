using System.Net;
using System.Text.Json;
using Application.Core;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // If there is no exception we continue with the request
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message); // We log the exception
                context.Response.ContentType = "application/json"; // We set the content type to json
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // We set the status code to 500

                var response = _env.IsDevelopment() // If the environment is development we return the stack trace otherwise we return Internal Server Error
                    ? new AppException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new AppException(context.Response.StatusCode, "Internal Server Error");

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }; // We set the options for the json serializer

                var json = JsonSerializer.Serialize(response, options); // We serialize the response to json

                await context.Response.WriteAsync(json); // We write the json to the response
            }
        }
    }
}
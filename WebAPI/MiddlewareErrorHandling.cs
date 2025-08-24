namespace WebAPI
{
    public class MiddlewareErrorHandling
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<MiddlewareErrorHandling> _logger;

        public MiddlewareErrorHandling(RequestDelegate next, ILogger<MiddlewareErrorHandling> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                httpContext.Response.ContentType = "application/json";

                await httpContext.Response.WriteAsJsonAsync(new
                {
                    Message = "An unexpected error has occurred.",
                });
            }
        }
    }
}

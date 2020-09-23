using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace LoggingMiddleware
{
    public class LogMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<LogMiddleware> _logger;

        public LogMiddleware(RequestDelegate next, ILogger<LogMiddleware> logger)
        {
            this.next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            //Ensure the request Microsoft.AspNetCore.Http.HttpRequest.Body can be read multiple times. Normally buffers request bodies in memory; writes requests larger than 30K bytes to disk.
            context.Request.EnableBuffering();

            // Get request body
            var request = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0; // return to start position

            // Prepare wrapped stream to read response
            var originalResponseBodyStream = context.Response.Body; // keep the original stream
            using var fakeResponseBodyStream = new MemoryStream();
            context.Response.Body = fakeResponseBodyStream; // give the our memory stream to others middlewares
            string response;

            // Invoke the next middleware
            try
            {
                await next(context);

                // Get response body from wrapped stream
                fakeResponseBodyStream.Seek(0, SeekOrigin.Begin);
                response = await new StreamReader(fakeResponseBodyStream).ReadToEndAsync();
                // Copy response to original stream
                fakeResponseBodyStream.Seek(0, SeekOrigin.Begin);
                await fakeResponseBodyStream.CopyToAsync(originalResponseBodyStream);
            }
            finally
            {
                // Ensures that the original stream has been recovered to be disposed of properly avoiding memory leak.
                context.Response.Body = originalResponseBodyStream;
            }

            // Get others request/response data
            var url = UriHelper.GetDisplayUrl(context.Request);
            var verbo = context.Request.Method;
            var status = context.Response.StatusCode;

            // Do anything
            _logger.LogInformation($"\nRequest:\n{verbo}: {url}\n{request}\nResponse: {status}\n{response}");
        }
    }

    public static class LogRequestMiddlewareExtensions
    {
        public static IApplicationBuilder UseLogRequest(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogMiddleware>();
        }
    }
}

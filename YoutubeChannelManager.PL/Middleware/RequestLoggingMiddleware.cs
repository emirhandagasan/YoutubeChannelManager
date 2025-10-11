using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace YoutubeChannelManager.PL.Middleware
{
    public class RequestLoggingMiddleware
   {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString();

            context.Items["RequestId"] = requestId;

            try
            {
                _logger.LogInformation(
                    "Incoming Request: {Method} {Path} | RequestId: {RequestId} | IP: {IP}",
                    context.Request.Method,
                    context.Request.Path,
                    requestId,
                    context.Connection.RemoteIpAddress?.ToString()
                );

                await _next(context);

                stopwatch.Stop();

                _logger.LogInformation(
                    "Completed Request: {Method} {Path} | Status: {StatusCode} | Duration: {Duration}ms | RequestId: {RequestId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    requestId
                );
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(
                    ex,
                    "Request Failed: {Method} {Path} | Duration: {Duration}ms | RequestId: {RequestId}",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds,
                    requestId
                );

                throw;
            }
        }
    }
}
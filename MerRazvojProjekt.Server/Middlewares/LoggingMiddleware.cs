using MerRazvojProjekt.Server.Data;
using MerRazvojProjekt.Server.Models;
using System;
using System.Diagnostics;

namespace MerRazvojProjekt.Server.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            var stopwatch = Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();

            var log = new RequestLog
            {
                HttpMethod = context.Request.Method,
                Path = context.Request.Path,
                ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                TimeStamp = DateTime.UtcNow
            };

            dbContext.RequestLogs.Add(log);
            await dbContext.SaveChangesAsync();
        }
    }
}

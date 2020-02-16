using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auction.Api.Middleware
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Auction.Api.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class RequestLoggerMiddleware
    {
        private readonly RequestDelegate next;

        public RequestLoggerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, ISimpleLogger dumbLogger)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            dumbLogger.LogInfo($"Handling request: {context.Request.Method} {context.Request.Path}");
            await this.next.Invoke(context);
            stopwatch.Stop();
            dumbLogger.LogInfo("Finished handling request [" + stopwatch.ElapsedMilliseconds + "ms]");
        }
    }

}

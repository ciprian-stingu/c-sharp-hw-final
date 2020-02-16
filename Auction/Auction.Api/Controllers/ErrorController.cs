namespace Hotels.Api.Controllers
{
    using System;
    using Auction.Api.Services;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private ISimpleLogger dumbLogger;

        public ErrorController(IWebHostEnvironment webHostEnvironment, ISimpleLogger dumbLogger)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.dumbLogger = dumbLogger;
        }

        // 6. handle all exceptions
        [Route("/error-local-development")]
        public IActionResult ErrorLocalDevelopment()
        {
            if (this.webHostEnvironment.EnvironmentName != "Development")
                throw new InvalidOperationException(
                    "This shouldn't be invoked in non-development environments.");

            var context = this.HttpContext.Features.Get<IExceptionHandlerFeature>();

            this.dumbLogger.LogInfo(context.Error.Message);

            return this.Problem(
                context.Error.StackTrace,
                title: context.Error.Message);
        }

        [Route("/error")]
        public IActionResult Error()
        {
            this.dumbLogger.LogInfo(this.HttpContext.Features.Get<IExceptionHandlerFeature>().Error.Message);

            return this.Problem();
        }
    }
}

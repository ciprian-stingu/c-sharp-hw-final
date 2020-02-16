namespace Auction.Api.Services
{
    using System;
    using Microsoft.Extensions.Logging;

    public interface ISimpleLogger
    {
        void LogInfo(string message);
    }

    public class DumbLogger : ISimpleLogger
    {
        private readonly Guid id;
        private readonly ILogger<DumbLogger> logger;

        public DumbLogger(ILogger<DumbLogger> logger)
        {
            this.id = Guid.NewGuid();
            
            this.logger = logger;
        }

        public void LogInfo(string message)
        {
            this.logger.LogInformation($"{this.id} : {message}");
        }
    }
}

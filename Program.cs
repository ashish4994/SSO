using System;

using CreditOne.Microservices.BuildingBlocks.Logger.FileLogger;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CreditOne.Microservices.Sso.API
{
    public class Program
    {
        #region Private Constants

        private const string MicroserviceHasStarted = "Ping Federate SSO Microservice has started.";
        private const string MicroserviceHasEnded = "Ping Federate SSO Microservice has stopped unexpectedly.";

        #endregion

        #region Public Methods

        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation(MicroserviceHasStarted);

            try
            {
                host.Run();
            }
            catch (Exception exception)
            {
                logger.LogCritical(exception, MicroserviceHasEnded);
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddFile();
                });

        #endregion
    }
}

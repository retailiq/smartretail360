using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SmartRetail360.Application;
using SmartRetail360.Infrastructure;
using SmartRetail360.Shared;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Infrastructure.Logging.Context;
using SmartRetail360.Infrastructure.Logging.Dispatcher;

namespace SmartRetail360.WorkerBootstrap.Hosting;

public static class WorkerHostFactory
{
    public static async Task<IHost> CreateAsync<TWorker>(string[] args) where TWorker : class, IHostedService
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
            .UseSerilog((context, services, loggerConfig) =>
            {
                loggerConfig
                    .MinimumLevel.Debug()
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext();
            })
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;
				
				services.AddLocalization(options => options.ResourcesPath = "Localization");
                services.AddSharedLayer(configuration);
                services.AddApplicationLayer();
                services.AddInfrastructureLayer(configuration);
                services.AddScoped<ILogContextAccessor, LogContextAccessor>();
                
                services.AddHostedService<TWorker>();
            })
            .Build();

        return host;
    }
}
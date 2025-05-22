using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SmartRetail360.Application;
using SmartRetail360.Infrastructure;
using SmartRetail360.Shared;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Infrastructure.Logging.Context;

namespace SmartRetail360.WorkerBootstrap.Hosting;

public static class WorkerHostFactory
{
    public static Task<IHost> CreateAsync<TWorker>(string[] args) where TWorker : class, IHostedService
    {
        var host = Host.CreateDefaultBuilder(args)
            .UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production")
            .ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment.EnvironmentName;
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
            .UseSerilog((context, loggerConfig) =>
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

        return Task.FromResult(host);
    }
}
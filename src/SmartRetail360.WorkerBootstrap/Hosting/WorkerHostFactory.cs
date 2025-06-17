using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SmartRetail360.Application;
using SmartRetail360.Execution;
using SmartRetail360.Infrastructure;
using SmartRetail360.Shared;
using SmartRetail360.Logging.Interfaces;
using SmartRetail360.Logging.Services.Context;
using SmartRetail360.Messaging;
using SmartRetail360.Notifications;
using SmartRetail360.Persistence;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Contexts;
using DependencyInjection = SmartRetail360.Logging.DependencyInjection;

namespace SmartRetail360.WorkerBootstrap.Hosting;

public static class WorkerHostFactory
{
    public static Task<IHost> CreateAsync<TWorker>(string[] args) where TWorker : class, IHostedService
    {
        var stopwatch = Stopwatch.StartNew();
        
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
				services.AddLocalization(options => options.ResourcesPath = GeneralConstants.Localization);
                services.AddShared(configuration);
                services.AddSharedContexts();            
                DependencyInjection.AddLogging(services);
                services.AddApplicationLayer();
                services.AddMessaging(configuration);
                services.AddExecution();
                services.AddNotifications();
                services.AddCaching(configuration);
                services.AddInfrastructureLayer(configuration);
                services.AddPersistence(configuration);
                services.AddScoped<ILogContextAccessor, LogContextAccessor>();
                
                services.AddHostedService<TWorker>();
            })
            .Build();
        
        stopwatch.Stop();
        Log.Information("[Startup] EmailWorker boot time: {Elapsed} ms", stopwatch.ElapsedMilliseconds);

        return Task.FromResult(host);
    }
}
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartRetail360.Infrastructure; // 用于 AddInfrastructureLayer
using SmartRetail360.Application;   // 用于 AddApplicationLayer
using SmartRetail360.Shared;        // 用于 AddSharedLayer

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // 引用 Server 的三层依赖
        services.AddLocalization(options => options.ResourcesPath = "Localization");
        services.AddSharedLayer(configuration);
        services.AddApplicationLayer();
        services.AddInfrastructureLayer(configuration);

        // 注册后台 Worker
        services.AddHostedService<EmailConsumerWorker>();
    })
    .Build();

await host.RunAsync();
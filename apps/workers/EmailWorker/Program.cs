// using System.Globalization;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Options;
// using Serilog;
// using SmartRetail360.Infrastructure; // 用于 AddInfrastructureLayer
// using SmartRetail360.Application;
// using SmartRetail360.Application.Interfaces.Logging;
// using SmartRetail360.Infrastructure.Logging.Context; // 用于 AddApplicationLayer
// using SmartRetail360.Shared;
// using SmartRetail360.Shared.Options; // 用于 AddSharedLayer
//
// var host = Host.CreateDefaultBuilder(args)
//     .ConfigureAppConfiguration((hostingContext, config) =>
//     {
//         config.SetBasePath(Directory.GetCurrentDirectory());
//         config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
//         config.AddEnvironmentVariables();
//     })
//     .UseSerilog((context, services, loggerConfig) => // ✅ 放在这里
//     {
//         loggerConfig
//             .MinimumLevel.Debug()
//             .ReadFrom.Configuration(context.Configuration)
//             .Enrich.FromLogContext();
//     })
//     .ConfigureServices((context, services) =>
//     {
//         var configuration = context.Configuration;
//         
//         // 引用 Server 的三层依赖
//         services.AddLocalization(options => options.ResourcesPath = "Localization");
//         services.AddSharedLayer(configuration);
//         services.AddApplicationLayer();
//         services.AddInfrastructureLayer(configuration);
//         services.AddScoped<ILogContextAccessor, LogContextAccessor>();
//
//
//         // 注册后台 Worker
//         services.AddHostedService<EmailConsumerWorker>();
//         
//     })
//     .Build();
//
// await host.RunAsync();


using SmartRetail360.WorkerBootstrap.Hosting;
using Microsoft.Extensions.Hosting;

var host = await WorkerHostFactory.CreateAsync<EmailConsumerWorker>(args);
await host.RunAsync();
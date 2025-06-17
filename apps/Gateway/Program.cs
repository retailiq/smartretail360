using System.Diagnostics;
using SmartRetail360.Gateway;
using SmartRetail360.Logging.Extensions;
using SmartRetail360.Shared.Constants;
using Serilog;

var stopwatch = Stopwatch.StartNew();

var builder = WebApplication.CreateBuilder(args);

// Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Serilog initialization
builder.Host.UseSmartRetailSerilog();
// Sentry initialization
builder.AddSmartRetailSentry();
// OTEL initialization
builder.Services.AddSmartRetailTelemetry(builder.Configuration, GeneralConstants.Sr360DotGateway);

// Register services from Startup
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Configure middleware pipeline
startup.Configure(app, builder.Environment);

stopwatch.Stop();
Log.Information("[Startup] Gateway boot time: {Elapsed} ms", stopwatch.ElapsedMilliseconds);

await app.RunAsync();
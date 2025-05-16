using Serilog;
using Serilog.Enrichers.Span;
using SmartRetail360.API;

var builder = WebApplication.CreateBuilder(args);

// Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Initialize Serilog
builder.Host.UseSerilog((context, services, loggerConfig) =>
{
    loggerConfig
        .MinimumLevel.Debug() // Set minimum log level
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithSpan()
        ;
});

// Configure Sentry
builder.WebHost.UseSentry(o =>
{
    o.Dsn = builder.Configuration["Sentry:Dsn"];
    o.SendDefaultPii = true;
    o.TracesSampleRate = 1.0;
    o.AttachStacktrace = true;
    o.Debug = true; // Enable debug mode
});

// Register services from Startup
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Configure middleware
startup.Configure(app, builder.Environment);

await app.RunAsync();
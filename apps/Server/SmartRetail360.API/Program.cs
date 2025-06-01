using Serilog;
using Serilog.Enrichers.Span;
using SmartRetail360.API;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Infrastructure.Data.Seed;
using SmartRetail360.Infrastructure.Data.Seed.AccessControl;
using StackExchange.Redis;

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
        .Enrich.WithSpan();
});

// Configure Sentry
builder.WebHost.UseSentry((WebHostBuilderContext context, Sentry.AspNetCore.SentryAspNetCoreOptions options) =>
{
    context.Configuration.GetSection("Sentry").Bind(options);
});

// Register services from Startup
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Configure middleware
startup.Configure(app, builder.Environment);

if (app.Environment.IsProduction())
{
    // prod: Sync Initialization
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();

    await SystemRoleDbRunner.RunAsync(db);
    await SystemRoleCacheRunner.RunAsync(db, redis);

    await AbacSeedRunner.RunAsync(db, redis);

    Console.WriteLine("[Startup] System role + ABAC seeding completed.");
}
else
{
    // dev: Async Initialization 
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        Task.Run(async () =>
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();

            try
            {
                await SystemRoleDbRunner.RunAsync(db);
                await SystemRoleCacheRunner.RunAsync(db, redis);

                await AbacSeedRunner.RunAsync(db, redis);

                Console.WriteLine("[Startup] System role seeding completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Startup] Seeding failed: {ex.Message}");
            }
        });
    });
}

await app.RunAsync();
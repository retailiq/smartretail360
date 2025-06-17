using System.Diagnostics;
using SmartRetail360.API;
using SmartRetail360.Logging.Extensions;
using SmartRetail360.Persistence.Data;
using SmartRetail360.Persistence.Seed;
using SmartRetail360.Persistence.Seed.AccessControl;
using StackExchange.Redis;
using Serilog;
using SmartRetail360.ABAC.Interfaces.AbacPolicyService;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Extensions;

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
builder.Services.AddSmartRetailTelemetry(builder.Configuration);

// Register services from Startup
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Configure middleware
startup.Configure(app, builder.Environment);

if (app.Environment.IsProduction())
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
        var policyCreator = scope.ServiceProvider.GetRequiredService<IAbacPolicyDefaultCreateService>();

        await SystemRoleDbRunner.RunAsync(db);
        await SystemRoleCacheRunner.RunAsync(db, redis);
        await AbacSeedRunner.RunAsync(db, redis);
        await TenantAccountDbRunner.RunAsync(db);
        await policyCreator.CreateDefaultPoliciesForTenantAsync(GeneralConstants.SystemTenantId, true);

        Log.Information("[Startup] System role + ABAC + Tenant account seeding completed.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "[Startup] Seeding failed in production mode");
    }
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
            var policyCreator = scope.ServiceProvider.GetRequiredService<IAbacPolicyDefaultCreateService>();

            try
            {
                await SystemRoleDbRunner.RunAsync(db);
                await SystemRoleCacheRunner.RunAsync(db, redis);
                await AbacSeedRunner.RunAsync(db, redis);
                await TenantAccountDbRunner.RunAsync(db);
                await policyCreator.CreateDefaultPoliciesForTenantAsync(GeneralConstants.SystemTenantId, true);

                Log.Information("[Startup] System role + ABAC + Tenant account seeding completed.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[Startup] Seeding failed in development mode");
            }
        });
    });
}

stopwatch.Stop();
Log.Information("[Startup] Server boot time: {Elapsed} ms", stopwatch.ElapsedMilliseconds);

await app.RunAsync();
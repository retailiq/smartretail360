using SmartRetail360.API;

var builder = WebApplication.CreateBuilder(args);

// Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Register services from Startup
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Configure middleware
startup.Configure(app, builder.Environment);

await app.RunAsync();
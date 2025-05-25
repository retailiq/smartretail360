// namespace SmartRetail360.API;
//
// public class Program
// {
//     public static void Main(string[] args)
//     {   
//         // Initialize the Builder
//         var builder = WebApplication.CreateBuilder(args);
//
//         // Register the services
//         builder.Services.AddAuthorization();
//         builder.Services.AddEndpointsApiExplorer();
//         builder.Services.AddSwaggerGen();
//         
//         // Build the application
//         var app = builder.Build();
//
//         // Configure the HTTP request pipeline
//         if (app.Environment.IsDevelopment())
//         {
//             app.UseSwagger();
//             app.UseSwaggerUI();
//         }
//         
//         // Register middleware and route mappings
//         app.UseHttpsRedirection();
//         app.UseAuthorization();
//         app.MapControllers(); 
//         
//         // Start the application
//         app.Run();
//     }
// }

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SmartRetail360.API;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    // Create and configure host builder
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
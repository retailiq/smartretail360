using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;
using SmartRetail360.API.Extensions;
using SmartRetail360.API.Middlewares;
using SmartRetail360.Application;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Infrastructure;
using SmartRetail360.Infrastructure.Logging.Context;
using SmartRetail360.Infrastructure.Middlewares;

namespace SmartRetail360.API;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    // Register services
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthorization();

        // 注册三层依赖
        services.AddApplicationLayer();
        services.AddInfrastructureLayer(Configuration);
        services.AddApiLayer(Configuration);

        // API 版本控制
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader()
            );
        });
        
        services.AddHttpContextAccessor();
        services.AddScoped<ILogContextAccessor, LogContextAccessor>();

        // Swagger API 文档版本化
        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
    }

    // Configure HTTP middleware pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // 本地化设置（必须在异常中间件前）
        var locOptions = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(locOptions.Value);

        // 读取 X-Locale 设置当前线程文化
        app.Use(async (context, next) =>
        {
            if (context.Request.Headers.TryGetValue("X-Locale", out var cultureValues))
            {
                var culture = cultureValues.ToString();
                if (!string.IsNullOrEmpty(culture))
                {
                    var cultureInfo = new CultureInfo(culture);
                    CultureInfo.CurrentCulture = cultureInfo;
                    CultureInfo.CurrentUICulture = cultureInfo;
                }
            }

            await next();
        });

        // 全局异常中间件
        app.UseMiddleware<ContextHeaderMiddleware>();
        app.UseMiddleware<LoggingContextMiddleware>();
        app.UseMiddleware<SentryContextMiddleware>();
        app.UseRequestLogging();
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var desc in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
                }
            });
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
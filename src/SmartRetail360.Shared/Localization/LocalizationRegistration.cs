using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;

namespace SmartRetail360.Shared.Localization;

public static class LocalizationRegistration
{
    public static IServiceCollection AddSmartRetailLocalization(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<AppOptions>(config.GetSection(GeneralConstants.App));
        services.AddLocalization(options => { options.ResourcesPath = GeneralConstants.Localization; });
        var appOptions = config.GetSection(GeneralConstants.App).Get<AppOptions>()!;
        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(appOptions.DefaultCulture);
            options.SupportedCultures = appOptions.SupportedCultures.Select(c => new CultureInfo(c)).ToList();
            options.SupportedUICultures = options.SupportedCultures;
        });
    
        return services;
    }

    public static IApplicationBuilder UseSmartRetailLocalization(this IApplicationBuilder app)
    {
        var locOptions = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(locOptions.Value);

        app.Use(async (context, next) =>
        {
            if (context.Request.Headers.TryGetValue("X-Locale", out var cultureValues))
            {
                var cultureStr = cultureValues.ToString();
                if (!string.IsNullOrWhiteSpace(cultureStr))
                {
                    var cultureInfo = new CultureInfo(cultureStr);
                    CultureInfo.CurrentCulture = cultureInfo;
                    CultureInfo.CurrentUICulture = cultureInfo;
                }
            }

            await next();
        });

        return app;
    }
}
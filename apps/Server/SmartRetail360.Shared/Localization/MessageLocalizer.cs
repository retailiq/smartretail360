using Microsoft.Extensions.Localization;
using SmartRetail360.Shared.Catalogs;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Shared.Localization;

public class MessageLocalizer
{
    private readonly IStringLocalizer _localizer;

    public MessageLocalizer(IStringLocalizerFactory factory)
    {
        var type = typeof(MessageLocalizer);
        _localizer = factory.Create("Messages", type.Assembly.GetName().Name!);
    }

    public string GetErrorMessage(int code)
    {
        var key = ErrorCatalog.GetKey(code);
        return _localizer[key];
    }
    
    public string GetLocalizedText(LocalizedTextKey key)
    {
        return _localizer[key.ToString()];
    }
}
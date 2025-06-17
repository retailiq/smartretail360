using Microsoft.Extensions.Localization;
using SmartRetail360.Shared.Catalogs;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Shared.Localization;

public class MessageLocalizer
{
    private readonly IStringLocalizer _localizer;

    public MessageLocalizer(IStringLocalizerFactory factory)
    {
        _localizer = factory.Create(GeneralConstants.Messages, GeneralConstants.Sr360DotShared);
    }
    public string GetErrorMessage(int code)
    {
        var key = ErrorCatalog.GetKey(code);
        return _localizer[key];
    }

    public string GetLocalizedText(LocalizedTextKey key, params object[]? args)
    {
        try
        {
            return args == null || args.Length == 0
                ? _localizer[key.ToString()]
                : _localizer[key.ToString(), args];
        }
        catch
        {
            return _localizer[key.ToString()]; // fallback
        }
    }
}
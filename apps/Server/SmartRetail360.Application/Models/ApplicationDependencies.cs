using SmartRetail360.Shared.Localization;

namespace SmartRetail360.Application.Models;

public class ApplicationDependencies
{
    public required MessageLocalizer Localizer { get; init; }
}
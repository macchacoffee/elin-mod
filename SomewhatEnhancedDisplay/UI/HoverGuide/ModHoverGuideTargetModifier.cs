namespace Macchacoffee.ElinMods.SomewhatEnhancedDisplay.UI.HoverGuide;

internal class ModHoverGuideTargetModifier(
    float? healthBarRatio = null,
    float? healthBarHPRatio = null,
    float? healthBarMPRatio = null)
{
    public double? HealthBarRatio { get; set; } = healthBarRatio;
    public double? HealthBarHPRatio { get; set; } = healthBarHPRatio;
    public double? HealthBarMPRatio { get; set; } = healthBarMPRatio;

    public bool HasManaBodyHealthBarPreview => HealthBarHPRatio is not null && HealthBarMPRatio is not null;
}

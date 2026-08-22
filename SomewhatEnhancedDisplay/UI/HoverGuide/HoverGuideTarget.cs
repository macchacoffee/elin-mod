namespace Macchacoffee.ElinMods.SomewhatEnhancedDisplay.UI.HoverGuide;

internal record HoverGuideTarget(
    string? Text1,
    string? Text2,
    Card? Card,
    HoverGuideTargetModifier? Modifier = null
);

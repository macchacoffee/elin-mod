namespace SomewhatEnhancedDisplay.UI.HoverGuide;

internal record ModHoverGuideTarget(
    string? Text1,
    string? Text2,
    Card? Card,
    ModHoverGuideTargetModifier? Modifier = null
);

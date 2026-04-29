namespace SomewhatEnhancedDisplay.UI.HoverGuide;

public static class ModCardHoverTextBuilder
{
    public static string BuildOtherCardsText(string hoverText, string otherCardsText)
    {
        return $"{hoverText}{otherCardsText.TagSize(ModUIUtil.ComputeFontSize(13))}";
    }
}

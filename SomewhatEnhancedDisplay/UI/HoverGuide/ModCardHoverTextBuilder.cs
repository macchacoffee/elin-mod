using System;
using System.Linq;

namespace SomewhatEnhancedDisplay.UI.HoverGuide;

public static class ModCardHoverTextBuilder
{
    private static readonly int PaddingHeight = 1;

    public static string BuildOtherCardsText(string hoverText, string otherCardsText)
    {
        return $"{hoverText}{otherCardsText.TagSize(ModUIUtil.ComputeFontSize(13))}";
    }

    public static string BuildHoverTextSection(params string?[] lines)
    {
        return string.Join(Environment.NewLine, lines.Where(l => !string.IsNullOrEmpty(l)));
    }

    public static string BuildHoverText(params string?[] sections)
    {
        return string.Join(
            $"{Environment.NewLine}{Environment.NewLine.TagSize(ModUIUtil.ComputeFontSize(PaddingHeight))}",
            sections.Where(t => !string.IsNullOrEmpty(t)));
    }
}

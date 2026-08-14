using System;
using System.Linq;

namespace SomewhatEnhancedDisplay.UI.HoverGuide;

internal static class ModCardHoverTextBuilder
{
    private static readonly int _paddingHeight = 1;

    public static string BuildOtherCardsText(string hoverText, string otherCardsText)
    {
        return $"{hoverText}{GetHoverTextOtherCards(otherCardsText)}";
    }

    public static string BuildHoverTextSection(string? line1, string? line2)
    {
        if (string.IsNullOrEmpty(line1))
        {
            return line2 ?? string.Empty;
        }
        if (string.IsNullOrEmpty(line2))
        {
            return line1 ?? string.Empty;
        }

        return string.Concat(line1, Environment.NewLine, line2);
    }

    public static string BuildHoverTextSection(params string?[] lines)
    {
        return string.Join(Environment.NewLine, lines.Where(l => !string.IsNullOrEmpty(l)));
    }

    public static string BuildHoverText(params string?[] sections)
    {
        return string.Join(
            $"{Environment.NewLine}{Environment.NewLine.TagSize(ModUIUtil.ComputeFontSize(_paddingHeight))}",
            sections.Where(t => !string.IsNullOrEmpty(t)));
    }

    private static string? GetHoverTextOtherCards(string otherCards)
    {
         return !string.IsNullOrEmpty(otherCards) ? otherCards.TagSize(ModUIUtil.ComputeFontSize(13)) : null;
    }
}

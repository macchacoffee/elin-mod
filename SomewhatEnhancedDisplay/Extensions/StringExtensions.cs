using UnityEngine;

namespace SomewhatEnhancedDisplay.Extensions;

public static class StringExtensions
{
    public static string TagColorNullable(this string s, Color? c)
    {
        return c is Color color ? s.TagColor(color) : s;
    }
}
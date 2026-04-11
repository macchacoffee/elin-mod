using UnityEngine;

namespace SomewhatEnhancedDisplay.Extensions;

public static class StringExtensions
{
    public static string TagColorNullable(this string text, Color? color)
    {
        return color is Color c ? text.TagColor(c) : text;
    }

    public static string TagColorIfNotEmptyNullable(this string text, Color? color)
    {
        return !text.IsEmpty() ? text.TagColorNullable(color) : text;
    }

    public static string TagSizeIfNotEmpty(this string text, int size)
    {
        return !text.IsEmpty() ? text.TagSize(size) : text;
    }
}
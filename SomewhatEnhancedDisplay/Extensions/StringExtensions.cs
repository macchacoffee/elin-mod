using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SomewhatEnhancedDisplay.Extensions;

public static class StringExtensions
{
    private static readonly Regex TagSizeRegex = new(@"(?<=<size=)(\d+)", RegexOptions.Compiled);

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

    public static string TagResize(this string text, Func<int, int> resizer)
    {
       return !text.IsEmpty() ? TagSizeRegex.Replace(text, m => resizer(int.Parse(m.Value)).ToString()) : text;
    }
}
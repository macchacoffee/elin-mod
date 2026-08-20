using System;
using System.Text.RegularExpressions;

using UnityEngine;

namespace Macchacoffee.ElinMods.SomewhatEnhancedDisplay.Extensions;

internal static class StringExtensions
{
    private static readonly Regex _tagTextRegex = new(@"(?<=>)[^<>]+(?=</[a-zA-Z0-9]+>)", RegexOptions.Compiled);
    private static readonly Regex _tagSizeRegex = new(@"(?<=<size=)(\d+)", RegexOptions.Compiled);

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
        return !text.IsEmpty() ? _tagSizeRegex.Replace(text, m => resizer(int.Parse(m.Value)).ToString()) : text;
    }

    public static string ReplaceTagTexts(this string text, Func<string, string> replacer, Func<string, string>? firstReplacer = null)
    {
        var i = 0;
        return !text.IsEmpty() ? _tagTextRegex.Replace(text, m => {
            return i++ == 0 && firstReplacer is not null ? firstReplacer(m.Value) : replacer(m.Value);
        }) : text;
    }
}

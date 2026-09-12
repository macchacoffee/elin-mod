using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Macchacoffee.ElinMods.ContainerPerformanceFix.Mod;

// Card.SecondaryCompareが使用する名前を、1回のCoreExtension.Sort内で再利用するキャッシュ。
internal static class SortNameCache
{
    [ThreadStatic]
    private static Dictionary<Card, string>? _cache;

    [ThreadStatic]
    private static int _depth;

    private static bool _enabled;

    internal static void Enable()
    {
        _enabled = true;
    }

    internal static void Begin()
    {
        if (!_enabled)
        {
            return;
        }

        if (_depth++ != 0)
        {
            return;
        }

        _cache ??= new(CardReferenceComparer.Instance);
        _cache.Clear();
    }

    internal static void End()
    {
        if (!_enabled || _depth <= 0 || --_depth != 0)
        {
            return;
        }

        _cache?.Clear();
    }

    internal static string GetName(Card card, NameStyle style, int num)
    {
        var cache = _cache;
        if (!_enabled || _depth <= 0 || cache is null)
        {
            return card.GetName(style, num);
        }

        // SecondaryCompareの正確な引数だけを対象とする。
        // vanillaのThing派生型は存在しないため、Mod由来の未知のoverrideは直接呼び出す。
        if (style != NameStyle.Full || num != 1 || card.GetType() != typeof(Thing))
        {
            return card.GetName(style, num);
        }

        if (cache.TryGetValue(card, out var name))
        {
            return name;
        }

        name = card.GetName(style, num);
        cache[card] = name;
        return name;
    }

    private sealed class CardReferenceComparer : IEqualityComparer<Card>
    {
        internal static readonly CardReferenceComparer Instance = new();

        public bool Equals(Card? x, Card? y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(Card obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}

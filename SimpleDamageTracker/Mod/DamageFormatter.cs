using System;

namespace Macchacoffee.ElinMods.SimpleDamageTracker.Mod;

internal static class DamageFormatter
{
    private const long Million = 1_000_000L;
    private const long Billion = 1_000_000_000L;
    private const long Trillion = 1_000_000_000_000L;
    private const long Quadrillion = 1_000_000_000_000_000L;
    private const long Quintillion = 1_000_000_000_000_000_000L;

    public static string Format(long damage, bool compacts)
    {
        if (!compacts || damage < Million)
        {
            return $"{damage:N0}";
        }
        if (damage >= Quintillion)
        {
            return FormatUnit(damage, Quintillion, "Qi");
        }
        if (damage >= Quadrillion)
        {
            return FormatUnit(damage, Quadrillion, "Qa");
        }
        if (damage >= Trillion)
        {
            return FormatUnit(damage, Trillion, "T");
        }
        if (damage >= Billion)
        {
            return FormatUnit(damage, Billion, "B");
        }
        return FormatUnit(damage, Million, "M");
    }

    private static string FormatUnit(long damage, long divisor, string suffix)
    {
        var value = (double)damage / divisor;
        var decimals = value switch
        {
            >= 100 => 0,
            >= 10 => 1,
            _ => 2,
        };
        var factor = decimals switch
        {
            0 => 1d,
            1 => 10d,
            _ => 100d,
        };
        var format = decimals switch
        {
            0 => "0",
            1 => "0.#",
            _ => "0.##",
        };

        value = Math.Floor(value * factor) / factor;
        return $"{value.ToString(format)}{suffix}";
    }
}

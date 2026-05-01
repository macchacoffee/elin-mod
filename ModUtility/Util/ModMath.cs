using System;

namespace ModUtility.Util;

public static class ModMath
{
    public static float Ceiling(float value, int digits)
    {
        var offset = Math.Pow(10, digits);
        return (float)(Math.Ceiling(value * offset) / offset);
    }

    public static double Ceiling(double value, int digits)
    {
        var offset = Math.Pow(10, digits);
        return Math.Ceiling(value * offset) / offset;
    }

    public static float Floor(float value, int digits)
    {
        var offset = Math.Pow(10, digits);
        return (float)(Math.Floor(value * offset) / offset);
    }

    public static double Floor(double value, int digits)
    {
        var offset = Math.Pow(10, digits);
        return Math.Floor(value * offset) / offset;
    }
}

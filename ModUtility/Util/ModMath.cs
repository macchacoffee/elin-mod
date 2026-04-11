using System;

namespace ModUtility.Util;

public static class ModMath
{
    public static float Ceiling(float value, int digits)
    {
        var offset = 10 ^ digits;
        return (float)Math.Ceiling(value * offset) / offset;
    }
}

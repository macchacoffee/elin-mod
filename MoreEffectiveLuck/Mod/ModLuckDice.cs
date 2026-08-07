
using System;

namespace MoreEffectiveLuck.Mod;

public class ModLuckDice<T>
{
    public Func<T> ResultFunc { get; }
    public Func<T, T, bool> ResultCompareFunc { get; }
    public bool IsPositive;
    private int ExtraRollCount { get; }
    public int RollCount
    {
        get
        {
            return 1 + ExtraRollCount;
        }
    }

    private ModLuckDice(Func<T> resultFunc, Func<T, T, bool> resultCompareFunc, int luck, int luckPerRoll, int maxRoll)
    {
        ResultFunc = resultFunc;
        ResultCompareFunc = resultCompareFunc;
        IsPositive = luck >= 0;
        ExtraRollCount = Math.Min(Math.Abs(luck / luckPerRoll) + (Math.Abs(luck % luckPerRoll) > EClass.rnd(luckPerRoll) ? 1 : 0), maxRoll);
    }

    public static ModLuckDice<T> Create(Func<T> resultFunc, Func<T, T, bool> resultCompareFunc, Card card, int? luckPerRoll = null, int? maxRoll = null)
    {
        return Create(resultFunc, resultCompareFunc, card.LUC, luckPerRoll, maxRoll);
    }

    public static ModLuckDice<T> Create(Func<T> resultFunc, Func<T, T, bool> resultCompareFunc, int luck, int? luckPerRoll = null, int? maxRoll = null)
    {
        return new(resultFunc, resultCompareFunc, luck, luckPerRoll ??= 100, maxRoll ??= 20);
    }

    public LuckDiceResult<T> Roll()
    {
        // 1回は必ずロールする
        var result = RollOnce();
        for (var i = 0; i < ExtraRollCount; i++)
        {
            var result2 = RollOnce();
            result = ResultCompareFunc(result2.Value, result.Value) ^ !IsPositive ? result2 : result;
        }
        return result;
    }

    public LuckDiceResult<T> RollOnce()
    {
        return new LuckDiceResult<T>(ResultFunc());
    }
}

public record LuckDiceResult<T>(T Value);

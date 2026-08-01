
using System;

namespace MoreEffectiveLuck.Utils;

public class LuckDice<T>(Func<T> resultFunc, Func<T, T, bool> resultCompareFunc, int luck)
{
    public Func<T> ResultFunc { get; } = resultFunc;
    public Func<T, T, bool> ResultCompareFunc { get; } = resultCompareFunc;
    private int ExtraRollCount { get; } = Math.Min(Math.Abs(luck / 100) + (Math.Abs(luck % 100) > EClass.rnd(100) ? 1 : 0), 19);
    public int RollCount
    {
        get
        {
            return 1 + ExtraRollCount;
        }
    }
    public bool IsPositive { get; } = luck >= 0;

    public static LuckDice<R> Create<R>(Func<R> resultFunc, Func<R, R, bool> resultCompareFunc, Card card)
    {
        return Create(resultFunc, resultCompareFunc, card.Evalue(SKILL.LUC));
    }

    public static LuckDice<R> Create<R>(Func<R> resultFunc, Func<R, R, bool> resultCompareFunc, int luck)
    {
        return new(resultFunc, resultCompareFunc, luck);
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

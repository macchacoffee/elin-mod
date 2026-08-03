
using System;

namespace MoreEffectiveLuck.Game;

public class LuckDice<T>
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

    private LuckDice(Func<T> resultFunc, Func<T, T, bool> resultCompareFunc, int luck, int luckPerRollCount, int maxRollCount)
    {
        ResultFunc = resultFunc;
        ResultCompareFunc = resultCompareFunc;
        IsPositive = luck >= 0;
        ExtraRollCount = Math.Min(Math.Abs(luck / luckPerRollCount) + (Math.Abs(luck % luckPerRollCount) > EClass.rnd(luckPerRollCount) ? 1 : 0), maxRollCount);
    }

    public static LuckDice<R> Create<R>(Func<R> resultFunc, Func<R, R, bool> resultCompareFunc, Card card, int? luckPerRollCount = null, int? maxRollCount = null)
    {
        return Create(resultFunc, resultCompareFunc, card.LUC, luckPerRollCount, maxRollCount);
    }

    public static LuckDice<R> Create<R>(Func<R> resultFunc, Func<R, R, bool> resultCompareFunc, int luck, int? luckPerRollCount = null, int? maxRollCount = null)
    {
        return new(resultFunc, resultCompareFunc, luck, luckPerRollCount ??= 100, maxRollCount ??= 20);
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

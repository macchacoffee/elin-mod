
using System;

namespace MoreEffectiveLuck.Utils;

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

    private LuckDice(Func<T> resultFunc, Func<T, T, bool> resultCompareFunc, int luck, int rollCountPerLuck, int maxRollCount)
    {
        ResultFunc = resultFunc;
        ResultCompareFunc = resultCompareFunc;
        IsPositive = luck >= 0;
        ExtraRollCount = Math.Min(Math.Abs(luck / rollCountPerLuck) + (Math.Abs(luck % rollCountPerLuck) > EClass.rnd(rollCountPerLuck) ? 1 : 0), maxRollCount);
    }

    public static LuckDice<R> Create<R>(Func<R> resultFunc, Func<R, R, bool> resultCompareFunc, Card card, int? rollCountPerLuck = null, int? maxRollCount = null)
    {
        return Create(resultFunc, resultCompareFunc, card.LUC, rollCountPerLuck, maxRollCount);
    }

    public static LuckDice<R> Create<R>(Func<R> resultFunc, Func<R, R, bool> resultCompareFunc, int luck, int? rollCountPerLuck = null, int? maxRollCount = null)
    {
        return new(resultFunc, resultCompareFunc, luck, rollCountPerLuck ??= 100, maxRollCount ??= 20);
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
        var result = new LuckDiceResult<T>(ResultFunc());
        Plugin.LogInfo($"RollOnce {result.Value}");
        return result;
        // return new LuckDiceResult<T>(ResultFunc());
    }
}

public record LuckDiceResult<T>(T Value);

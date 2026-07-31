
using System;

namespace MoreEffectiveLuck.Utils;

public class LuckDice
{
    public int RollCount { get; }
    public bool IsPositive { get; }
    public int? Result { get; private set; }

    public LuckDice() : this(EClass.pc) {}

    public LuckDice(Chara chara)
    {
        var luck = chara.Evalue(SKILL.LUC);
        RollCount = Math.Min(1 + Math.Abs(luck / 100) + (Math.Abs(luck % 100) > EClass.rnd(100) ? 1 : 0), 20);
        IsPositive = luck >= 0;
    }

    public void UpdateResult(int result)
    {
        if (Result is int prev)
        {
            Result = IsPositive ? Math.Max(prev, result) : Math.Min(prev, result);
        }
        else
        {
            Result = result;
        }
    }
}

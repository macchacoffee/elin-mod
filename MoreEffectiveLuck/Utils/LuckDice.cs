
using System;

namespace MoreEffectiveLuck.Utils;

public static class LuckDice
{
    public static int RollCount()
    {
        return RollCount(EClass.pc);
    }

    public static int RollCount(Chara chara)
    {
        var luck = Math.Max(chara.Evalue(SKILL.LUC), 0);
        return Math.Min(1 + luck / 100 + ((luck % 100) > EClass.rnd(100) ? 1 : 0), 20);
    }
}

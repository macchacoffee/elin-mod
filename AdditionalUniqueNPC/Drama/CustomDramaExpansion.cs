using System;
using System.Collections.Generic;
using Macchacoffee.ElinMods.ModUtility.Logging;

namespace Macchacoffee.ElinMods.AdditionalUniqueNPC.Drama;

internal static class CustomDramaExpansion
{
    [ElinDramaActionInvoke]
    private static bool mc_aun_move_to_pc(DramaManager dm, Dictionary<string, string> line)
    {
        var chara = dm.GetChara(line["actor"]);

        if (chara.currentZone != EClass._zone)
        {
            chara.MoveZone(EClass._zone);
        }

        chara.MoveImmediate(
            EClass.pc.pos.GetNearestPoint(
                allowBlock: false,
                allowChara: false));

        return true;
    }

    [ElinDramaActionInvoke]
    private static bool mc_aun_if_pc_faction(
        DramaManager dm,
        Dictionary<string, string> line,
        bool isPositive = true)
    {
        var chara = dm.GetChara(line["actor"]);

        return chara.IsPCFaction ^ !isPositive;
    }

    [ElinDramaActionInvoke]
    private static bool mc_aun_drain_mana_pc(
        DramaManager dm,
        Dictionary<string, string> line,
        float ratio)
    {
        var chara = dm.GetChara(line["actor"]);
        Chara target = EClass.pc;
        if (target == null || ratio <= 0)
        {
            return false;
        }

        var before = target.mana.value;
        if (before <= 0)
        {
            return false;
        }

        var amount = Math.Min(
            before,
            Math.Max(1, (int)(target.mana.max * ratio))
        );

        target.mana.Mod(-amount);

        var drained = Math.Min(
            before,
            Math.Max(0, before - target.mana.value)
        );

        chara.mana.Mod(drained);

        return true;
    }
}

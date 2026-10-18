using System.Collections.Generic;

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

    [ElinDramaActionInvoke("nodiscard")]
    private static bool mc_aun_if_pc_faction(
        DramaManager dm,
        Dictionary<string, string> line,
        bool isPositive = true)
    {
        var chara = dm.GetChara(line["actor"]);

        return chara.IsPCFaction ^ !isPositive;
    }
}

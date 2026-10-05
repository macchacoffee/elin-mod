using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

namespace Macchacoffee.ElinMods.NoSpikyPetting.Patches;

[HarmonyPatch(typeof(AI_Mofu))]
internal static class AI_MofuPatch
{
    [HarmonyTranspiler]
    [HarmonyPatch(MethodType.Enumerator)]
    [HarmonyPatch(nameof(AI_Mofu.Run), [])]
    private static IEnumerable<CodeInstruction> MoveNext_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator)
    {
        // 変更前
        // mofu.ExistsOnMap
        // 変更後
        // AI_MofuPatch.IsSafeTargetOnMap(mofu)
        var matcher = new CodeMatcher(instructions, generator);
        var mofu = AccessTools.Field(typeof(AI_Mofu), nameof(AI_Mofu.mofu));
        var existsOnMap = AccessTools.PropertyGetter(typeof(Card), nameof(Card.ExistsOnMap));
        var isSafeTargetOnMap = AccessTools.Method(typeof(AI_MofuPatch), nameof(IsSafeTargetOnMap));

        // 移動前と移動後の確認でトゲトゲの判定を行うようにする。
        for (var i = 0; i < 2; i++)
        {
            matcher.MatchEndForward(
                new CodeMatch(OpCodes.Ldfld, mofu),
                new CodeMatch(OpCodes.Callvirt, existsOnMap)
            );
            matcher.SetAndAdvance(OpCodes.Call, isSafeTargetOnMap);
        }
        return matcher.InstructionEnumeration();
    }

    private static bool IsSafeTargetOnMap(Chara target)
    {
        return target.ExistsOnMap && target.Evalue(FEAT.featSpike) <= 0;
    }
}

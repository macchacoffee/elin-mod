using System.Collections.Generic;
using System.Reflection;

using HarmonyLib;

namespace Macchacoffee.ElinMods.NoSpikyPetting.Patches;

[HarmonyPatch]
internal static class AIActMofuTargetPatch
{
    [HarmonyTargetMethods]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        // AI_Idle.Run / AI_Mofu.GetMofu の撫でる対象を選ぶラムダ関数。
        yield return AccessTools.Method(typeof(AI_Idle), "<Run>b__12_4");
        yield return AccessTools.Method(typeof(AI_Mofu), "<GetMofu>b__3_0");
    }

    [HarmonyPostfix]
    private static void Postfix(Chara __0, ref bool __result)
    {
        __result = __result && __0.Evalue(FEAT.featSpike) <= 0;
    }
}

using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;

namespace DiningSpotSignTweaks.Patches;

[HarmonyPatch(typeof(Chara))]
public static class CharaPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Chara.CanReplace), [typeof(Chara)])]
    private static void CanReplace_Postfix(Chara __instance, ref bool __result, Chara c)
    {
        if (__instance.IsPC || c.IsPC || __instance.IsHostile(c) || c.IsMultisize || !c.trait.CanBePushed || c.isRestrained || c.noMove)
        {
            return;
        }
        if (__instance.ai.child?.child is not AI_Eat aiEat)
        {
            return;
        }
        if (aiEat.child is not AI_Goto)
        {
            return;
        }

        // 特定のAI階層にAI_EatとAI_Gotoが存在する場合、
        // 食事のために食堂の立札を目指して移動しているとみなし、押しのけ可能とする
        __result = true;
    }
}

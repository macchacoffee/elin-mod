using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;

namespace NPCGotoBehaviorTweaks.Patches;

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
        if (!__instance.ai.IsMoveAI)
        {
            return;
        }

        // 移動AIの場合は押しのけ可能とする
        __result = true;
    }
}

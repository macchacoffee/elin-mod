using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;
using SomewhatEnhancedDisplay.Extensions;
using SomewhatEnhancedDisplay.UI;

namespace SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(Card))]
public static class CardPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Card.GetHoverText2), [])]
    private static void GetHoverText2_Postfix(Card __instance, ref string __result)
    {
        if (__instance is not Thing thing)
        {
            return;
        }

        var traitText = thing.trait.GetHoverText();
        __result = BuildHoverText2(__result, traitText, thing);
    }

    private static string BuildHoverText2(string text, string traitText, Thing thing)
    {
        return ModThingHoverTextBuilder.BuildHoverText2(thing, text, traitText);
    }
}

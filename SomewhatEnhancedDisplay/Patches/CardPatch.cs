using System.Reflection;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;
using Macchacoffee.ElinMods.SomewhatEnhancedDisplay.Extensions;
using Macchacoffee.ElinMods.SomewhatEnhancedDisplay.UI;
using Macchacoffee.ElinMods.SomewhatEnhancedDisplay.UI.HoverGuide;

namespace Macchacoffee.ElinMods.SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(Card))]
internal static class CardPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original) && ModContext.Config.EnableHoverGuide.Value;
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

    private static int ComputeFontSize(int size)
    {
        // フォントサイズを微調整する。
        return ModUIUtil.ComputeFontSize(size - 1);
    }

    private static string BuildHoverText2(string text, string traitText, Thing thing)
    {
        text = text.TagResize(ComputeFontSize);
        traitText = traitText.TagResize(ComputeFontSize);
        return ModThingHoverTextBuilder.BuildHoverText2(thing, text, traitText);
    }
}

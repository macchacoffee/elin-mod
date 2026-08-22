using System.Reflection;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;
using Macchacoffee.ElinMods.StarweaversMoongateQuickFix.Mod;

namespace Macchacoffee.ElinMods.StarweaversMoongateQuickFix.Patches;

[HarmonyPatch(typeof(UIList))]
internal static class UIListPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(UIList.List), [typeof(bool)])]
    private static void List_Postfix(UIList __instance)
    {
        ModMoongatePaging.UpdatePageControls(__instance);
    }
}

using System.Reflection;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;
using Macchacoffee.ElinMods.StarweaversMoongatePaging.Mod;

namespace Macchacoffee.ElinMods.StarweaversMoongatePaging.Patches;

[HarmonyPatch(typeof(UIList))]
internal static class UIListPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(UIList.List), [typeof(bool)])]
    private static void List_Postfix(UIList __instance)
    {
        MoongatePaging.UpdatePageControls(__instance);
    }
}

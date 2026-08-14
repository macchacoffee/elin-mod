using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;
using SimpleDamageTracker.Mod;

namespace SimpleDamageTracker.Patches;

[HarmonyPatch(typeof(ButtonRoster))]
internal static class ButtonRosterPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ButtonRoster.SetChara), [typeof(Chara)])]
    private static void SetChara_Postfix(ButtonRoster __instance, Chara c)
    {
        var display = __instance.GetComponent<ModDamageDisplay>() ?? __instance.gameObject.AddComponent<ModDamageDisplay>();
        display.Bind(__instance, c);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ButtonRoster.Refresh), [])]
    private static void Refresh_Postfix(ButtonRoster __instance)
    {
        __instance.GetComponent<ModDamageDisplay>()?.RefreshIfNeeded();
    }
}

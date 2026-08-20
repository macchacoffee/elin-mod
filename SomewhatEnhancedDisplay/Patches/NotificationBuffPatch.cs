using System.Reflection;
using HarmonyLib;
using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(NotificationBuff))]
internal static class NotificationBuffPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original) && ModContext.Config.EnableStatusNotification.Value;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NotificationBuff.OnRefresh), [])]
    private static void OnRefresh_Postfix(NotificationBuff __instance)
    {
        if (!EClass.debug.showExtra)
        {
            __instance.text += $" {__instance.condition.value}";
        }
    }
}

using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;

namespace SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(NotificationBuff))]
public static class NotificationBuffPatch
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

using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;

namespace SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(NotificationCondition))]
internal static class NotificationConditionPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original) && ModContext.Config.EnableStatusNotification.Value;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NotificationCondition.OnRefresh), [])]
    private static void OnRefresh_Postfix(NotificationCondition __instance)
    {
        if (!EClass.debug.showExtra)
        {
            __instance.text += $" {__instance.condition.value}";
        }
    }
}

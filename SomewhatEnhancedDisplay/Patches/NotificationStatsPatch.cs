using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;

namespace SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(NotificationStats))]
internal static class NotificationStatsPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original) && ModContext.Config.EnableStatusNotification.Value;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NotificationStats.OnRefresh), [])]
    private static void OnRefresh_Postfix(NotificationStats __instance)
    {
        if (EClass.debug.showExtra)
        {
            return;
        }

        var stats = __instance.stats();
        if (!stats.GetText().IsEmpty())
        {
            __instance.text += $"({stats.GetValue()})";
        }
    }
}

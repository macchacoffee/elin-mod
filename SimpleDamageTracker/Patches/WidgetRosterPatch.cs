using System.Reflection;
using HarmonyLib;
using Macchacoffee.ElinMods.ModUtility.Patch;
using Macchacoffee.ElinMods.SimpleDamageTracker.UI.Config;
using YKF;

namespace Macchacoffee.ElinMods.SimpleDamageTracker.Patches;

[HarmonyPatch(typeof(WidgetRoster))]
internal static class WidgetRosterPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(WidgetRoster.OnSetContextMenu), [typeof(UIContextMenu)])]
    private static void OnSetContextMenu_Postfix(WidgetRoster __instance, UIContextMenu m)
    {
        m.AddButton(ModConsts.SourceId.ModName, () =>
        {
            YK.CreateLayer<ModLayerConfig>();
        });
    }
}

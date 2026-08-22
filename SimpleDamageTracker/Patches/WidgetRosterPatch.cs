using System.Reflection;

using HarmonyLib;
using YKF;

using Macchacoffee.ElinMods.ModUtility.Patch;
using Macchacoffee.ElinMods.SimpleDamageTracker.UI.Config;

namespace Macchacoffee.ElinMods.SimpleDamageTracker.Patches;

[HarmonyPatch(typeof(WidgetRoster))]
internal static class WidgetRosterPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(WidgetRoster.OnSetContextMenu), [typeof(UIContextMenu)])]
    private static void OnSetContextMenu_Postfix(UIContextMenu m)
    {
        m.AddButton(ModConsts.SourceId.ModName, () =>
        {
            YK.CreateLayer<ModLayerConfig>();
        });
    }
}

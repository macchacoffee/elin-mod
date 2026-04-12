using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;
using SomewhatEnhancedDisplay.UI.HoverGuide;
using YKF;

namespace SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(Widget))]
public static class WidgetPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Widget.OnSetContextMenu), [typeof(UIContextMenu)])]
    private static void OnSetContextMenu_Postfix(Widget __instance, UIContextMenu m)
    {
        if (__instance is not WidgetMouseover widget || m == null)
        {
            return;
        }

        m.AddButton(ModConsts.SourceId.ModName, () =>
        {
            YK.CreateLayer<ModLayerConfig>();
        });
    }
}

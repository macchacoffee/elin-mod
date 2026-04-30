using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;
using SomewhatEnhancedDisplay.UI.HoverGuide.Config;
using UnityEngine;
using YKF;

namespace SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(HotItemContext))]
public static class HotItemContextPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(HotItemContext.Show), [typeof(string), typeof(Vector3)])]
    private static void Show_Postfix(string id, Vector3 pos)
    {
        if (EClass.ui.contextMenu.currentMenu is null || id != "system")
        {
            return;
        }

        var uiContextMenu = EClass.ui.contextMenu.currentMenu.AddOrGetChild("tool");
        uiContextMenu.AddButton(ModConsts.SourceId.ModName, () =>
        {
            YK.CreateLayer<ModLayerConfig, ModLayerConfigContext>(new());
        });
    }
}

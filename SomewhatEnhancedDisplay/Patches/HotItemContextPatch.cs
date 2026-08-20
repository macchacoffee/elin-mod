using System.Reflection;
using HarmonyLib;
using Macchacoffee.ElinMods.ModUtility.Patch;
using Macchacoffee.ElinMods.SomewhatEnhancedDisplay.UI.HoverGuide.Config;
using UnityEngine;
using YKF;

namespace Macchacoffee.ElinMods.SomewhatEnhancedDisplay.Patches;

[HarmonyPatch(typeof(HotItemContext))]
internal static class HotItemContextPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original) && ModContext.Config.EnableHoverGuide.Value;
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

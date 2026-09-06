using System;
using System.Reflection;

using Emmersive.Contexts;
using HarmonyLib;
using YKF;

namespace Macchacoffee.ElinMods.EmmersiveIndividualBackgrounds.Patches;

[HarmonyPatch]
internal static class TabEmmersiveBaseBuildPromptCardPatch
{
    [HarmonyTargetMethod]
    private static MethodBase BuildPromptCard_TargetMethod()
    {
        return AccessTools.Method("Emmersive.Components.TabEmmersiveBase:BuildPromptCard")
            ?? throw new MissingMethodException("Emmersive.Components.TabEmmersiveBase", "BuildPromptCard");
    }

    [HarmonyPrefix]
    private static void BuildPromptCard_Prefix(ref string path)
    {
        var chara = IndividualBackgroundState.CurrentChara;
        if (chara is null || path != IndividualBackgroundState.GetCommonPath(chara))
        {
            return;
        }

        if (IndividualBackgroundState.IsIndividualMode(chara))
        {
            path = IndividualBackgroundState.GetIndividualPath(chara);
        }
    }

    [HarmonyPostfix]
    private static void BuildPromptCard_Postfix(YKLayout __result)
    {
        var chara = IndividualBackgroundState.CurrentChara;
        if (chara is null || __result is null)
        {
            return;
        }

        var selectedIndex = IndividualBackgroundState.IsIndividualMode(chara) ? 1 : 0;
        var modeRow = __result.Horizontal();
        var dropdown = modeRow.Dropdown(
            IndividualBackgroundState.ModeLabels,
            index => ChangeMode(chara, index),
            selectedIndex);
        dropdown.WithWidth(160);
        modeRow.transform.SetSiblingIndex(1);
    }

    private static void ChangeMode(Chara chara, int index)
    {
        var individual = index == 1;
        if (IndividualBackgroundState.IsIndividualMode(chara) == individual)
        {
            return;
        }

        IndividualBackgroundState.SetIndividualMode(chara, individual);
        LayerPanelAccess.ScheduleReopenPreservingScrollPosition();
    }
}

[HarmonyPatch]
internal static class TabEmmersiveBaseLayoutPatch
{
    [HarmonyTargetMethod]
    private static MethodBase OnLayoutConfirm_TargetMethod()
    {
        return AccessTools.Method("Emmersive.Components.TabEmmersiveBase:OnLayoutConfirm")
            ?? throw new MissingMethodException("Emmersive.Components.TabEmmersiveBase", "OnLayoutConfirm");
    }

    [HarmonyPostfix]
    private static void OnLayoutConfirm_Postfix()
    {
        if (IndividualBackgroundState.ConsumeRefreshRequest())
        {
            LayerPanelAccess.Reopen();
        }
    }
}

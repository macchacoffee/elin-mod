using System;
using System.Reflection;

using HarmonyLib;

namespace Macchacoffee.ElinMods.EmmersiveIndividualBackgrounds.Patches;

[HarmonyPatch]
internal static class TabCharaPromptPatch
{
    [HarmonyTargetMethod]
    private static MethodBase AddCharaButton_TargetMethod()
    {
        return AccessTools.Method("Emmersive.Components.TabCharaPrompt:AddCharaButton")
            ?? throw new MissingMethodException("Emmersive.Components.TabCharaPrompt", "AddCharaButton");
    }

    [HarmonyPrefix]
    private static void AddCharaButton_Prefix(Chara chara, out Chara? __state)
    {
        __state = IndividualBackgroundState.CurrentChara;
        IndividualBackgroundState.CurrentChara = chara;
        IndividualBackgroundState.Register(chara);
    }

    [HarmonyFinalizer]
    private static Exception? AddCharaButton_Finalizer(Exception? __exception, Chara? __state)
    {
        IndividualBackgroundState.CurrentChara = __state;
        return __exception;
    }
}

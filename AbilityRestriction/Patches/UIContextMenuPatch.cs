using System;
using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;
using AbilityRestriction.Mod;

namespace AbilityRestriction.Patches;

[HarmonyPatch(typeof(UIContextMenu))]
public static class UIContextMenuPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(UIContextMenu.AddButton), [typeof(string), typeof(Action), typeof(bool)])]
    private static void AddButton_Postfix(UIContextMenu __instance, string idLang = "", Action? action = null, bool hideAfter = true)
    {
        if (BaseListPeoplePatch.TargetChara is null || idLang != "changeName")
        {
            return;
        }
        var chara = BaseListPeoplePatch.TargetChara;
        BaseListPeoplePatch.TargetChara = null;
        __instance.AddButton(ModConsts.SourceId.RestrictAbilities, ModAbilityRestriction.BuildSettingLayer(chara));
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(UIContextMenu.Show), [])]
    private static void AddButton_Postfix(UIContextMenu __instance)
    {
        if (!ModContext.Config.EnableViaInteraction.Value)
        {
            return;
        }
        if (EClass.scene.mouseTarget.card is not Chara chara || __instance.name != "ContextInteraction(Clone)")
        {
            return;
        }
        if (!ModAbilityRestriction.CanRestrictAbility(chara))
        {
            return;
        }
        __instance.AddButton(ModConsts.SourceId.RestrictAbilities, ModAbilityRestriction.BuildSettingLayer(chara));
    }
}

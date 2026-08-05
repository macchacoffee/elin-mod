using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using ModUtility.Patch;
using AbilityRestriction.Config;

namespace AbilityRestriction.Patches;

[HarmonyPatch(typeof(UIContextMenu))]
public static class UIContextMenuPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
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

        var originalActs = ModContext.OriginalActStorage.GetActs(chara);
        var deniedAbility = ModContext.Config.GetDeniedAbility(chara.uid);
        if (deniedAbility is null)
        {
            deniedAbility = new ModConfigDeniedAbility();
        }

        __instance.AddButton(ModNames.RestrictAbilities.Text, () =>
        {
            EClass.ui.AddLayer<LayerList>()
               .SetListCheck(originalActs,
               (item) => item.act.Name + (item.pt ? $" ({ModNames.Party.Text})" : ""),
               (item, _) =>
               {
                   var act = new ModConfigDeniedAct(item);
                   if (deniedAbility.Contains(act))
                   {
                       deniedAbility.Remove(act);
                   }
                   else
                   {
                       deniedAbility.Add(act);
                   }

                   if (deniedAbility.IsEmpty())
                   {
                       ModContext.Config.RemoveDeniedAbility(chara.uid);
                   }
                   else
                   {
                       ModContext.Config.SetDeniedAbility(chara.uid, deniedAbility);
                   }
                   chara.ability.Refresh();
               }, (buttonPairList) =>
               {
                   foreach (var buttonPair in buttonPairList)
                   {
                       var button = (buttonPair.component as ItemGeneral)!.button1;
                       var item = buttonPair.obj as ActList.Item;
                       var act = new ModConfigDeniedAct(item!);
                       button.SetCheck(!deniedAbility.Contains(act));
                       button.GetComponent<CanvasGroup>().enabled = false;
                   }
               })
            .SetHeader(ModNames.RestrictAbilities.Text)
           .SetSize();
        });  
    }
}

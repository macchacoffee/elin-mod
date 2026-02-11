using System;
using HarmonyLib;
using UnityEngine;

namespace AbilityRestriction.Patches;

[HarmonyPatch(typeof(UIContextMenu))]
public static class UIContextMenuPatchPatch
{
    [HarmonyPatch(nameof(UIContextMenu.AddButton), [typeof(string), typeof(Action), typeof(bool)]), HarmonyPostfix]
    public static void AddButton_Postfix(UIContextMenu __instance, string idLang = "", Action? action = null, bool hideAfter = true)
    {
        if (BaseListPeoplePatch.TargetChara == null || idLang != "changeName")
        {
            return;
        }

        var chara = BaseListPeoplePatch.TargetChara;
        BaseListPeoplePatch.TargetChara = null;

        var originalActs = Mod.originalActStorage.GetActs(chara);
        var deniedAbility = Mod.config.GetDeniedAbility(chara.uid);
        if (deniedAbility == null)
        {
            deniedAbility = new ModDeniedAbility();
        }

        __instance.AddButton(ModNames.restrictAbilities.Text, () =>
        {
            EClass.ui.AddLayer<LayerList>()
               .SetListCheck(originalActs,
               (item) => item.act.Name,
               (item, _) =>
               {
                   if (deniedAbility.Contains(item.act.id))
                   {
                       deniedAbility.Remove(item.act.id);
                   }
                   else
                   {
                       deniedAbility.Add(item.act.id);
                   }

                   if (deniedAbility.IsEmpty())
                   {
                       Mod.config.RemoveDeniedAbility(chara.uid);
                   }
                   else
                   {
                       Mod.config.SetDeniedAbility(chara.uid, deniedAbility);
                   }

                   chara.ability.Refresh();
               }, (buttonPairList) =>
               {
                   foreach (var buttonPair in buttonPairList)
                   {
                       var button = (buttonPair.component as ItemGeneral)!.button1;
                       var item = buttonPair.obj as ActList.Item;
                       button.SetCheck(!deniedAbility.Contains(item!.act.id));
                       button.GetComponent<CanvasGroup>().enabled = false;
                   }
               })
            .SetHeader(ModNames.restrictAbilities.Text)
           .SetSize();
        });
    }
}

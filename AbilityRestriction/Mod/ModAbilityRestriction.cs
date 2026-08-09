using UnityEngine;
using AbilityRestriction.Config;
using System;

namespace AbilityRestriction.Mod;

public static class ModAbilityRestriction
{
    public static bool CanRestrictAbility(Chara chara)
    {
        return !chara.IsPC && (ModContext.Config.EnableForAllNPC.Value || chara.IsHomeMember());
    }

    public static Action BuildSettingLayer(Chara chara)
    {
        var originalActs = ModContext.OriginalActStorage.GetActs(chara);
        var deniedAbility = ModContext.WorldConfig.GetDeniedAbility(chara.uid);
        deniedAbility ??= new ModConfigDeniedAbility();

        return () => {
            EClass.ui.AddLayer<LayerList>()
                .SetListCheck(originalActs,
                (item) => item.act.Name + (item.pt ? $" ({ModConsts.SourceId.Party.lang()})" : ""),
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
                        ModContext.WorldConfig.RemoveDeniedAbility(chara.uid);
                    }
                    else
                    {
                        ModContext.WorldConfig.SetDeniedAbility(chara.uid, deniedAbility);
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
            .SetHeader(ModConsts.SourceId.RestrictAbilities)
            .SetSize();
        };
    }
}

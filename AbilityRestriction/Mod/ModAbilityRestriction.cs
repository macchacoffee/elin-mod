using UnityEngine;
using AbilityRestriction.Config;
using System;

namespace AbilityRestriction.Mod;

internal static class ModAbilityRestriction
{
    public static bool CanRestrictAbility(Chara chara)
    {
        return !chara.IsPC && (ModContext.Config.EnableForAllNPC.Value || chara.IsHomeMember());
    }

    public static Action BuildSettingLayer(Chara chara)
    {
        var originalActs = ModContext.OriginalActStorage.GetActs(chara);

        return () => {
            EClass.ui.AddLayer<LayerList>()
                .SetListCheck(originalActs,
                item => item.act.Name + (item.pt ? $" ({ModConsts.SourceId.Party.lang()})" : ""),
                (item, _) =>
                {
                    var act = new ModConfigDeniedAct(item);
                    var deniedAbility = ModContext.WorldConfig.GetDeniedAbility(chara.uid);
                    if (deniedAbility?.Contains(act) == true)
                    {
                        ModContext.WorldConfig.RemoveDeniedAct(chara.uid, act);
                    }
                    else
                    {
                        ModContext.WorldConfig.AddDeniedAct(chara.uid, act);
                    }

                    chara.ability.Refresh();
                    if (chara.ai is GoalCombat goal && goal.abilities != null)
                    {
                        // GoalCombatが持つアビリティ一覧が構築済みであれば再構築し、
                        // 戦闘中でもアビリティ禁止設定の変更が反映されるようにする
                        goal.BuildAbilityList();
                    }
                }, buttonPairList =>
                {
                    foreach (var buttonPair in buttonPairList)
                    {
                        var button = (buttonPair.component as ItemGeneral)!.button1;
                        var item = buttonPair.obj as ActList.Item;
                        var act = new ModConfigDeniedAct(item!);
                        var deniedAbility = ModContext.WorldConfig.GetDeniedAbility(chara.uid);

                        button.SetCheck(deniedAbility?.Contains(act) != true);
                        button.GetComponent<CanvasGroup>().enabled = false;
                    }
                })
            .SetHeader(ModConsts.SourceId.RestrictAbilities)
            .SetSize();
        };
    }
}

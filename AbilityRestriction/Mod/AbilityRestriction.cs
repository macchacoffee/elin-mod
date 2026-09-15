using System;
using System.Linq;

using UnityEngine;

using Macchacoffee.ElinMods.AbilityRestriction.Config;

namespace Macchacoffee.ElinMods.AbilityRestriction.Mod;

internal static class AbilityRestriction
{
    public static bool CanRestrictAbility(Chara chara)
    {
        return !chara.IsPC && (ModContext.Config.EnableForAllNPC.Value || chara.IsHomeMember());
    }

    public static bool HasUnderlyingAbility(Chara chara, int actId)
    {
        // Charaに設定されたアビリティを確認する。
        if (chara._listAbility?.Any(a => Mathf.Abs(a) == actId) == true)
        {
            return true;
        }

        // SourceCharaに設定されたアビリティを確認する。
        foreach (var act in chara.source.actCombat)
        {
            var alias = act.Split('/')[0];

            if (chara.MainElement != Element.Void
                && EClass.sources.elements.alias[alias].aliasRef == "mold")
            {
                alias += chara.MainElement.source.alias.Replace("ele", "");
            }

            if (ACT.dict[alias].source.id == actId)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsAbilityRestricted(Chara chara, ModConfigDeniedAct act)
    {
        return ModContext.WorldConfig.GetDeniedAbility(chara.uid)?.Contains(act) == true;
    }

    public static void RestrictAbility(Chara chara, ModConfigDeniedAct act)
    {
        ModContext.WorldConfig.AddDeniedAct(chara.uid, act);
    }

    public static void UnrestrictAbility(Chara chara, ModConfigDeniedAct act)
    {
        ModContext.WorldConfig.RemoveDeniedAct(chara.uid, act);
    }

    public static Action BuildSettingLayer(Chara chara)
    {
        var originalActs = ModContext.OriginalActStorage.GetActs(chara);

        return () =>
        {
            EClass.ui.AddLayer<LayerList>()
                .SetListCheck(originalActs,
                item => item.act.Name + (item.pt ? $" ({ModConsts.SourceId.Party.lang()})" : ""),
                (item, _) =>
                {
                    var act = new ModConfigDeniedAct(item);
                    if (IsAbilityRestricted(chara, act))
                    {
                        UnrestrictAbility(chara, act);
                    }
                    else
                    {
                        RestrictAbility(chara, act);
                    }

                    chara.ability.Refresh();
                    if (chara.ai is GoalCombat goal && goal.abilities is not null && goal.owner is not null)
                    {
                        // GoalCombatが持つアビリティ一覧が構築済みであれば再構築し、
                        // 戦闘中でもアビリティ禁止設定の変更が反映されるようにする。
                        goal.BuildAbilityList();
                    }
                }, buttonPairList =>
                {
                    foreach (var buttonPair in buttonPairList)
                    {
                        var button = (buttonPair.component as ItemGeneral)!.button1;
                        var item = buttonPair.obj as ActList.Item;
                        var act = new ModConfigDeniedAct(item!);

                        button.SetCheck(!IsAbilityRestricted(chara, act));
                        button.GetComponent<CanvasGroup>().enabled = false;
                    }
                })
            .SetHeader(ModConsts.SourceId.RestrictAbilities)
            .SetSize();
        };
    }
}

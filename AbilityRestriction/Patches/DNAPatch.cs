using System.Linq;
using System.Reflection;

using HarmonyLib;
using Macchacoffee.ElinMods.ModUtility.Patch;
using UnityEngine;

namespace Macchacoffee.ElinMods.AbilityRestriction.Patches;

[HarmonyPatch(typeof(DNA))]
internal static class DNAPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(DNA.GetInvalidAction), [typeof(Chara)])]
    private static void GetInvalidAction_Postfix(DNA __instance, Chara c, ref Element __result)
    {
        if (__result is not null)
        {
            return;
        }

        for (int i = 0; i < __instance.vals.Count; i += 2)
        {
            var id = __instance.vals[i];
            if (IsRestrectedAbility(c, id) && HasUnderlyingAbility(c, id))
            {
                // 制限中の習得済みアビリティのElementを復元し、
                // 遺伝子の注入時のアビリティ重複チェックが正しく機能するようにする
                __result = Element.Create(id, __instance.vals[i + 1]);
                return;
            }
        }
    }

    private static bool IsRestrectedAbility(Chara chara, int actId)
    {
        // GetInvalidAction()の実装に揃えるため、DNAのアビリティ重複チェック時はIDだけで比較する
        return ModContext.WorldConfig.GetDeniedAbility(chara.uid)?.ContainsId(actId) == true;
    }

    private static bool HasUnderlyingAbility(Chara chara, int actId)
    {
        // Charaに設定されたアビリティを確認する。
        if (chara._listAbility?.Any(x => Mathf.Abs(x) == actId) == true)
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
}

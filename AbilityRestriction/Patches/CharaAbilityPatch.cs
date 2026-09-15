using System.Reflection;

using HarmonyLib;
using UnityEngine;

using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.AbilityRestriction.Patches;

[HarmonyPatch(typeof(CharaAbility))]
internal static class CharaAbilityPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CharaAbility.Refresh), [])]
    private static void Refresh_Postfix(CharaAbility __instance)
    {
        var owner = __instance.owner;

        var deniedAbility = ModContext.WorldConfig.GetDeniedAbility(owner.uid);
        if (deniedAbility is null)
        {
            ModContext.OriginalActStorage.RemoveActs(owner);
            return;
        }

        // charaが元々持っているアビリティの情報が必要になるため、保存しておく
        ModContext.OriginalActStorage.SetActs(owner, __instance.list.items);
        // 禁止されているアビリティをcharaのCharaAbilityから削除する
        __instance.list.items.RemoveAll(item => deniedAbility.Contains(new(item)));
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(CharaAbility.Add), [typeof(int), typeof(int), typeof(bool)])]
    private static void Add_Prefix(CharaAbility __instance, int id, int chance, bool pt)
    {
        var owner = __instance.owner;
        Mod.AbilityRestriction.UnrestrictAbility(owner, new(id, pt));
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CharaAbility.Remove), [typeof(int)])]
    private static void Remove_Postfix(CharaAbility __instance, int id)
    {
        var owner = __instance.owner;
        var actId = Mathf.Abs(id);
        var pt = id < 0;
        if (!Mod.AbilityRestriction.HasUnderlyingAbility(owner, actId))
        {
            Mod.AbilityRestriction.UnrestrictAbility(owner, new(actId, pt));
        }
    }
}

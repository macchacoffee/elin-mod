using System.Linq;
using System.Reflection;
using AbilityRestriction.Config;
using HarmonyLib;
using ModUtility.Patch;

namespace AbilityRestriction.Patches;

[HarmonyPatch(typeof(CharaAbility))]
internal static class CharaAbilityPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

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
        // charaが忘れたアビリティを禁止アビリティの設定から削除する
        deniedAbility.IntersectWith(__instance.list.items.Select(item => new ModConfigDeniedAct(item)));
        // 禁止されているアビリティをcharaのCharaAbilityから削除する
        __instance.list.items.RemoveAll(item => deniedAbility.Contains(new ModConfigDeniedAct(item)));
    }
}

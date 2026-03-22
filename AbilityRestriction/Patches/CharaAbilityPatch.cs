using System.Linq;
using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;

namespace AbilityRestriction.Patches;

[HarmonyPatch(typeof(CharaAbility))]
public static class CharaAbilityPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CharaAbility.Refresh), [])]
    private static void Refresh_Postfix(CharaAbility __instance)
    {
        var owner = __instance.owner;

        var deniedAbility = Mod.Config.GetDeniedAbility(owner.uid);
        if (deniedAbility is null)
        {
            Mod.OriginalActStorage.RemoveActs(owner);
            return;
        }

        // charaが元々持っているアビリティの情報が必要になるため、保存しておく
        Mod.OriginalActStorage.SetActs(owner, __instance.list.items);
        // charaが忘れたアビリティを禁止アビリティの設定から削除する
        deniedAbility.IntersectWith(__instance.list.items.Select(item => new ModDeniedAct(item)));
        // 禁止されているアビリティをcharaのCharaAbilityから削除する
        __instance.list.items.RemoveAll(item => deniedAbility.Contains(new ModDeniedAct(item)));
    }
}

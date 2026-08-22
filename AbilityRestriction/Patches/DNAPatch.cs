using System.Reflection;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;

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
        if (__result != null)
        {
            return;
        }

        for (int i = 0; i < __instance.vals.Count; i += 2)
        {
            var id = __instance.vals[i];
            // GetInvalidAction()の実装に揃えるため、DNAのアビリティ重複チェック時はIDだけで比較する
            if (ModContext.WorldConfig.GetDeniedAbility(c.uid)?.ContainsId(id) == true)
            {
                // 制限しているアビリティのElementを復元し、
                // 遺伝子の注入時のアビリティ重複チェックが正しく機能するようにする
                __result = Element.Create(id, __instance.vals[i + 1]);
                return;
            }
        }
    }
}

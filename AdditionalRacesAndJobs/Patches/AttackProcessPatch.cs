using System.Reflection;

using HarmonyLib;
using UnityEngine;

using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.AdditionalRacesAndJobs.Patches;

[HarmonyPatch(typeof(AttackProcess))]
internal static class AttackProcessPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(
        nameof(AttackProcess.Prepare),
        [typeof(Chara), typeof(Thing), typeof(Card), typeof(Point), typeof(int), typeof(bool)])]
    private static void Prepare_Postfix(AttackProcess __instance)
    {
        if (__instance.isThrow)
        {
            return;
        }

        var featLevel = __instance.CC.Evalue(ModConsts.SourceNumericId.FeatMonk);
        if (featLevel <= 0)
        {
            return;
        }

        var attackMultiplier = !__instance.IsRanged && (__instance.IsMartial || __instance.IsMartialWeapon)
            ? ModConsts.Value.MonkMartialAttackMultiplier
            : -ModConsts.Value.MonkNonMartialAttackMultiplier;

        var multiplier = Mathf.Max(1 + featLevel * attackMultiplier, 0);
        __instance.dMulti *= multiplier;
        __instance.toHit = (long)(__instance.toHit * multiplier);
    }
}

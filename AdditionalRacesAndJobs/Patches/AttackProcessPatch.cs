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

        var multiplier = !__instance.IsRanged && (__instance.IsMartial || __instance.IsMartialWeapon)
            ? ModConsts.Value.MonkMartialAttackMultiple
            : -ModConsts.Value.MonkNonMartialAttackMultiple;

        __instance.dMulti += featLevel * multiplier;
        __instance.toHit = (long)Mathf.Max(__instance.toHit * (1 + featLevel * multiplier), 0);
    }
}

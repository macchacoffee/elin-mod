using System.Collections.Generic;
using System.Reflection;

using HarmonyLib;

using Macchacoffee.ElinMods.AbilityRestriction;
using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.AdditionalUniqueNPC.Patches;

[HarmonyPatch(typeof(Chara))]
internal static class CharaPatch
{
    private static readonly PatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    private static readonly HashSet<string> OnBanishTargetCharaIds = [
        ModConsts.SourceId.Misalie,
    ];

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Chara.OnBanish), [])]
    private static void OnBanish_Postfix(Chara __instance)
    {
        if (!OnBanishTargetCharaIds.Contains(__instance.id) || !__instance.IsGlobal)
        {
            return;
        }

        var uniqueData = __instance.c_uniqueData;
        if (uniqueData != null &&
            EClass.game.spatials.map.ContainsKey(uniqueData.uidZone))
        {
            return;
        }

        if (!ModUtil.TryGetContent<CustomCharaContent>(
                $"Chara/{__instance.id}", out var content) ||
            content.spawnZones.Count != 1)
        {
            return;
        }

        var spawnZone = content.spawnZones[0];
        if (spawnZone == "*")
        {
            return;
        }

        var zone = ModUtil.FindZoneByFullName(spawnZone);
        if (zone == null)
        {
            return;
        }

        __instance.SetHomeZone(zone);
        __instance.MoveZone(
            zone, ZoneTransition.EnterState.RandomVisit);
    }
}

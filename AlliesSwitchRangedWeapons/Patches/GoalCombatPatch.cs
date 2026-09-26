using HarmonyLib;

using Macchacoffee.ElinMods.AlliesSwitchRangedWeapons.Mod;

namespace Macchacoffee.ElinMods.AlliesSwitchRangedWeapons.Patches;

[HarmonyPatch(typeof(GoalCombat))]
internal static class GoalCombatPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(GoalCombat.TryUseRanged), [typeof(int)])]
    private static void TryUseRanged_Prefix(GoalCombat __instance)
    {
        RangedWeaponSwitcher.TrySwitchRangedWeapon(__instance.owner, __instance.tc);
    }
}

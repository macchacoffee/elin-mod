using HarmonyLib;

namespace Macchacoffee.ElinMods.GravityGunSoundOverlapFix.Patches;

[HarmonyPatch(typeof(SoundManager))]
internal static class SoundManagerPatch
{
    private const string GravityGunSoundId = "attack_gun_gravity";

    [HarmonyPostfix]
    [HarmonyPatch(nameof(SoundManager.GetData), [typeof(string)])]
    private static void GetData_Postfix(string id, SoundData? __result)
    {
        if (id != GravityGunSoundId || __result == null)
        {
            return;
        }

        __result.allowMultiple = false;
        __result.skipIfPlaying = true;
    }
}

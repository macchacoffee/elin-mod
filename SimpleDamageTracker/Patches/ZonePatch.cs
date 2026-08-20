using System.Reflection;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;

namespace Macchacoffee.ElinMods.SimpleDamageTracker.Patches;

[HarmonyPatch(typeof(Zone))]
internal static class ZonePatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Zone.Activate), [])]
    private static void Activate_Prefix(Zone __instance)
    {
        if (EClass.game is not Game game)
        {
            return;
        }
        if (game.isLoading || game.activeZone != __instance)
        {
            ModContext.DamageTracker.Reset();
        }
    }

    private static void OnDamage(Card card, Card origin, long dmg)
    {
        if (origin?.Chara is not Chara originChara || !originChara.IsPCParty)
        {
            return;
        }
        ModContext.DamageTracker.AddDamage(originChara.uid, dmg);
    }
}

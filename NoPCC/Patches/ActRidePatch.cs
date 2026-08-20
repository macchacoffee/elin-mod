using System.Reflection;

using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Patch;
using Macchacoffee.ElinMods.NoPCC.Mod;

namespace Macchacoffee.ElinMods.NoPCC.Patches;

[HarmonyPatch(typeof(ActRide))]
internal static class ActRidePatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ActRide.Ride), [typeof(Chara), typeof(Chara), typeof(bool), typeof(bool)])]
    private static void Ride_Postfix(Chara host, Chara t, bool parasite = false, bool talk = true)
    {
        if (!host.IsPC)
        {
            return;
        }

        ModPCRenderer.Update();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ActRide.Unride), [typeof(Chara), typeof(Chara), typeof(bool)])]
    private static void Unride_Postfix(Chara host, Chara mount, bool talk = true)
    {
        if (!host.IsPC)
        {
            return;
        }

        ModPCRenderer.Update();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ActRide.Unride), [typeof(Chara), typeof(bool), typeof(bool)])]
    private static void Unride_Postfix(Chara host, bool parasite = false, bool talk = true)
    {
        if (!host.IsPC)
        {
            return;
        }

        ModPCRenderer.Update();
    }
}

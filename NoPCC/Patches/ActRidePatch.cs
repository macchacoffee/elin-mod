using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;

namespace NoPCC.Patches;

[HarmonyPatch(typeof(ActRide))]
public static class ActRidePatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyPatch(nameof(ActRide.Ride), [typeof(Chara), typeof(Chara), typeof(bool), typeof(bool)]), HarmonyPostfix]
    private static void Ride_Postfix(Chara host, Chara t, bool parasite = false, bool talk = true)
    {
        if (!host.IsPC)
        {
            return;
        }

        ModPCRenderer.Update();
    }

    [HarmonyPatch(nameof(ActRide.Unride), [typeof(Chara), typeof(Chara), typeof(bool)]), HarmonyPostfix]
    private static void Unride_Postfix(Chara host, Chara mount, bool talk = true)
    {
        if (!host.IsPC)
        {
            return;
        }

        ModPCRenderer.Update();
    }

    [HarmonyPatch(nameof(ActRide.Unride), [typeof(Chara), typeof(bool), typeof(bool)]), HarmonyPostfix]
    private static void Unride_Postfix(Chara host, bool parasite = false, bool talk = true)
    {
        if (!host.IsPC)
        {
            return;
        }

        ModPCRenderer.Update();
    }
}

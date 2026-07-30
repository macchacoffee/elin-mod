using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using HarmonyLib;
using ModUtility.Patch;

namespace FactionEffectInPCInventory.Patches;

[HarmonyPatch(typeof(ThingContainer))]
public static class ThingContainerPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ThingContainer.OnAdd), [typeof(Thing)])]
    private static void OnAdd_Postfix(ThingContainer __instance, Thing t)
    {
        Plugin.LogInfo($"OnAdd_Postfix {__instance} {t.Name}");
        EClass.pc.faction.charaElements.OnEquip(t);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ThingContainer.OnRemove), [typeof(Thing)])]
    private static void OnRemove_Postfix(ThingContainer __instance, Thing t)
    {
        Plugin.LogInfo($"OnRemove_Postfix {__instance} {t.Name}");
        EClass.pc.faction.charaElements.OnUnequip(t);
    }
}

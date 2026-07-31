using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;
using SomewhatEnhancedDisplay.Extensions;

namespace FactionEnchantInInventory.Patches;

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
        if (!__instance.owner.GetRootCard().IsPCFaction)
        {
            return;
        }
        EClass.pc.faction.charaElements.OnAdd(t);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ThingContainer.OnRemove), [typeof(Thing)])]
    private static void OnRemove_Postfix(ThingContainer __instance, Thing t)
    {
        if (!__instance.owner.GetRootCard().IsPCFaction)
        {
            return;
        }
        EClass.pc.faction.charaElements.OnRemove(t);
    }
}

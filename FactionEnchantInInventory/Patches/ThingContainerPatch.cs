using System.Reflection;
using HarmonyLib;
using Macchacoffee.ElinMods.ModUtility.Patch;
using Macchacoffee.ElinMods.FactionEnchantInInventory.Extensions;

namespace Macchacoffee.ElinMods.FactionEnchantInInventory.Patches;

[HarmonyPatch(typeof(ThingContainer))]
internal static class ThingContainerPatch
{
    private static readonly ModPatchTarget _patchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return _patchTarget.IsPatchable(original);
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

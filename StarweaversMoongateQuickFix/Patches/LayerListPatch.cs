using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Macchacoffee.ElinMods.StarweaversMoongateQuickFix.Mod;

namespace Macchacoffee.ElinMods.StarweaversMoongateQuickFix.Patches;

[HarmonyPatch]
internal static class LayerListSetList2Patch
{
    private static MethodBase TargetMethod()
    {
        var method = AccessTools
            .GetDeclaredMethods(typeof(LayerList))
            .Single(x => x.Name == nameof(LayerList.SetList2) && x.IsGenericMethodDefinition);
        return method.MakeGenericMethod(typeof(MapMetaData));
    }

    private static void Prefix(LayerList __instance, ref ICollection<MapMetaData> __0)
    {
        if (!ModMoongatePaging.IsOpening)
        {
            return;
        }
        if (__0 is not List<MapMetaData> source)
        {
            return;
        }

        ModMoongatePaging.Attach(__instance, source, ref __0);
    }

    private static void Postfix()
    {
        if (!ModMoongatePaging.IsOpening)
        {
            return;
        }

        ModMoongatePaging.SetupPageButton();
    }
}

[HarmonyPatch(typeof(LayerList), nameof(LayerList.OnKill))]
internal static class LayerListOnKillPatch
{
    private static void Prefix(LayerList __instance)
    {
        ModMoongatePaging.Detach(__instance);
    }
}

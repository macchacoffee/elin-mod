using Emmersive.API.Services;
using Emmersive.Contexts;
using HarmonyLib;

namespace EmmersiveIndividualBackgrounds.Patches;

[HarmonyPatch(typeof(ResourceFetch))]
internal static class ResourceFetchPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ResourceFetch.GetActiveResource), [typeof(ResourceKey), typeof(bool)])]
    private static bool GetActiveResource_Prefix(ResourceKey __0, ref string __result)
    {
        if (!IndividualBackgroundState.TryGetRegisteredChara(__0, out var chara)
            || IndividualBackgroundState.HasCustomResource(__0))
        {
            return true;
        }

        __result = new BackgroundContext(chara).Build() as string ?? string.Empty;
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ResourceFetch.RemoveCustomResource), [typeof(ResourceKey)])]
    private static void RemoveCustomResource_Postfix(ResourceKey __0)
    {
        if (!IndividualBackgroundState.TryGetRegisteredChara(__0, out var chara)
            || IndividualBackgroundState.HasCustomResource(__0))
        {
            return;
        }

        IndividualBackgroundState.SetIndividualMode(chara, false);
        IndividualBackgroundState.RequestRefresh();
    }
}

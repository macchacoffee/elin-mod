using Emmersive.API.Services;
using Emmersive.Contexts;
using HarmonyLib;

namespace EmmersiveIndividualBackgrounds.Patches;

[HarmonyPatch(typeof(BackgroundContext))]
internal static class BackgroundContextPatch
{
    private static readonly AccessTools.FieldRef<BackgroundContext, Chara> _charaRef =
        AccessTools.FieldRefAccess<BackgroundContext, Chara>("<chara>P");

    [HarmonyPrefix]
    [HarmonyPatch(nameof(BackgroundContext.Build), [])]
    private static bool Build_Prefix(BackgroundContext __instance, ref object? __result)
    {
        var chara = _charaRef(__instance);
        var key = new ResourceKey(IndividualBackgroundState.GetIndividualPath(chara));
        if (!IndividualBackgroundState.HasCustomResource(key))
        {
            return true;
        }

        var background = ResourceFetch.GetActiveResource(key, true);
        __result = string.IsNullOrEmpty(background) || background == "em_ui_non_provided"
            ? null
            : background;
        return false;
    }
}

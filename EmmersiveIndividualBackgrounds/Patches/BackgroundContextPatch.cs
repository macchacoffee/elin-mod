using System.Runtime.CompilerServices;
using Emmersive.API.Services;
using Emmersive.Contexts;
using HarmonyLib;

namespace EmmersiveIndividualBackgrounds.Patches;

[HarmonyPatch(typeof(BackgroundContext))]
internal static class BackgroundContextPatch
{
    private static readonly ConditionalWeakTable<BackgroundContext, Chara> _charas = new();

    [HarmonyPostfix]
    [HarmonyPatch(MethodType.Constructor, [typeof(Chara)])]
    private static void Constructor_Postfix(BackgroundContext __instance, Chara __0)
    {
        _charas.Add(__instance, __0);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(BackgroundContext.Build), [])]
    private static bool Build_Prefix(BackgroundContext __instance, ref object? __result)
    {
        if (!_charas.TryGetValue(__instance, out var chara))
        {
            return true;
        }

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

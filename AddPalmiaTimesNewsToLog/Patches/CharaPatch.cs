using System.Reflection;
using HarmonyLib;
using ModUtility.Patch;

namespace AddPalmiaTimesNewsToLog.Patches;

[HarmonyPatch(typeof(Chara))]
public static class CharaPatch
{
    private static readonly ModPatchTarget PatchTarget = new();

    [HarmonyPrepare]
    private static bool Prepare(MethodBase? original)
    {
        return PatchTarget.IsPatchable(original);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Chara.Tick), [])]
    private static void Tick_Prefix(Chara __instance)
    {
        if (!__instance.IsPC)
        {
            return;
        }

        var newsList = Mod.NewsFeeder.GetRandomNews();
        if (newsList.Count == 0)
        {
            return;
        }

        Mod.NewsFeeder.IsNewsReady = false;
        foreach (var news in newsList)
        {
            Msg.Say(news.TagColor(Mod.Config.LogColor));
        }
    }
}

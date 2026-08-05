using System.Reflection;
using AddPalmiaTimesNewsToLog.Config;
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

        ModContext.NewsFeeder.RequestFetch();
        var newsList = ModContext.NewsFeeder.GetRandomNews();
        if (newsList.Count == 0)
        {
            return;
        }

        ModContext.NewsFeeder.InvalidateNews();
        foreach (var news in newsList)
        {
            LogNews(news);
        }
    }

    private static void LogNews(string news)
    {
        switch (ModContext.Config.LogTarget)
        {
            case ModLogTarget.Log:
                Msg.Say(news.TagColor(ModContext.Config.LogColor));
                break;
            case ModLogTarget.Feed:
                // TODO 1件ずつ表示する
                WidgetFeed.Instance?.Nerun(news);
                break;
        }
    }
}

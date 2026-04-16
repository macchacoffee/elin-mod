using System.Reflection;
using System.Runtime.CompilerServices;
using AddPalmiaTimesNewsToLog.Config;
using BepInEx;
using HarmonyLib;

namespace AddPalmiaTimesNewsToLog;

public static class PluginInfo
{
    public const string Guid = "maccha-coffee.add-palmia-times-news-to-log";
    public const string Name = "Add Palmia Times News To Log";
    public const string Version = "1.0.0";
}

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
internal class Plugin : BaseUnityPlugin
{
    internal static Plugin? Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginInfo.Guid);
    }

    private void Update()
    {
        if (!EClass.core.IsGameStarted)
        {
            return;
        }

        if (Mod.Config.Enable && !Mod.NewsFeeder.IsRunning)
        {
            Mod.NewsFeeder.StartFetching();
        }
        else if (!Mod.Config.Enable && Mod.NewsFeeder.IsRunning)
        {
            Mod.NewsFeeder.StopFetching();
        }
    }

    internal static void LogDebug(object message, [CallerMemberName] string caller = "")
    {
        Instance?.Logger.LogDebug($"[{caller}] {message}");
    }

    internal static void LogInfo(object message)
    {
        Instance?.Logger.LogInfo(message);
    }

    internal static void LogError(object message)
    {
        Instance?.Logger.LogError(message);
    }
}

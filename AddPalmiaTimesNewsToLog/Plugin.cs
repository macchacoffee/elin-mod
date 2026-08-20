using System;
using System.Reflection;

using BepInEx;
using HarmonyLib;

namespace Macchacoffee.ElinMods.AddPalmiaTimesNewsToLog;

internal static class PluginInfo
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
        try
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginInfo.Guid);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to apply harmony patch: {ex}");
        }
    }

    private void Update()
    {
        if (!EClass.core.IsGameStarted)
        {
            return;
        }

        if (ModContext.Config.Enable && !ModContext.NewsFeeder.IsRunning)
        {
            ModContext.NewsFeeder.StartFetching();
        }
        else if (!ModContext.Config.Enable && ModContext.NewsFeeder.IsRunning)
        {
            ModContext.NewsFeeder.StopFetching();
        }
    }

    internal static void LogDebug(object message)
    {
        Instance?.Logger.LogDebug(message);
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

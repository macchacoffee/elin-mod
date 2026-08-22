using System;
using System.Reflection;

using BepInEx;
using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Logging;

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
    private void Awake()
    {
        ModLog.Initialize(Logger);
        try
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginInfo.Guid);
        }
        catch (Exception ex)
        {
            ModLog.Error($"Failed to apply harmony patch: {ex}");
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
}

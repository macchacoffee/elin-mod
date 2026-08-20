using System;
using System.Reflection;

using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.External.ModConfigGUI;

namespace Macchacoffee.ElinMods.MoreEffectiveLuck;

internal static class PluginInfo
{
    public const string Guid = "maccha-coffee.more-effective-luck";
    public const string Name = "More Effective Luck";
    public const string Version = "1.0.0";
}

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
internal class Plugin : BaseUnityPlugin
{
    internal static Plugin? Instance { get; private set; }
    internal static Harmony? Harmony { get; private set; }
    private static ConfigFile? ConfigFile { get; set; }

    private void Awake()
    {
        Instance = this;
        ConfigFile = ModContext.BindConfig();
        Harmony = new Harmony(PluginInfo.Guid);
        try
        {
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to apply harmony patch: {ex}");
        }
    }

    private void Start()
    {
        ModConfigGUISupport.ResisterConfig(PluginInfo.Guid, PluginInfo.Name, ConfigFile!);
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

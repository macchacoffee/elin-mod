using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ModUtility.External.ModConfigGUI;

namespace AbilityRestriction;

internal static class PluginInfo
{
    public const string Guid = "maccha-coffee.ability-restriction";
    public const string Name = "Ability Restriction";
    public const string Version = "1.2.1";
}

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
internal class Plugin : BaseUnityPlugin
{
    internal static Plugin? Instance { get; private set; }
    private static ConfigFile? ConfigFile { get; set; }

    private void Awake()
    {
        Instance = this;
        ConfigFile = ModContext.BindConfig();
        try
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginInfo.Guid);
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

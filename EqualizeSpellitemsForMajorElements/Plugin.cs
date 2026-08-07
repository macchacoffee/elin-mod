using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ModUtility.External.ModConfigGUI;

namespace EqualizeSpellitemsForMajorElements;

public static class PluginInfo
{
    public const string Guid = "maccha-coffee.equalize-spellitems-for-major-elements";
    public const string Name = "Equalize Spellitems for Major Elements";
    public const string Version = "1.0.1";
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

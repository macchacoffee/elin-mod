using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ModUtility.External.ModConfigGUI;
using SomewhatEnhancedDisplay.Patches;
using SomewhatEnhancedDisplay.UI;

namespace SomewhatEnhancedDisplay;

public static class PluginInfo
{
    public const string Guid = "maccha-coffee.somewhat-enhanced-display";
    public const string Name = "Somewhat Enhanced Display";
    public const string Version = "1.0.0";
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
            Harmony.CreateAndPatchAll(typeof(GameIOPatch), PluginInfo.Guid);
            Harmony.CreateAndPatchAll(typeof(GamePatch), PluginInfo.Guid);
            if (ModContext.Config.EnableHoverGuide.Value)
            {
                Harmony.CreateAndPatchAll(typeof(CardPatch), PluginInfo.Guid);
                Harmony.CreateAndPatchAll(typeof(CharaPatch), PluginInfo.Guid);
                Harmony.CreateAndPatchAll(typeof(HotItemContextPatch), PluginInfo.Guid);
                Harmony.CreateAndPatchAll(typeof(ThingPatch), PluginInfo.Guid);
                Harmony.CreateAndPatchAll(typeof(WidgetMouseoverPatch), PluginInfo.Guid);
            }
            if (ModContext.Config.EnableDNA.Value)
            {
                Harmony.CreateAndPatchAll(typeof(DNAPatch), PluginInfo.Guid);
            }
            if (ModContext.Config.EnableEnchant.Value)
            {
                Harmony.CreateAndPatchAll(typeof(ElementPatch), PluginInfo.Guid);
            }
            if (ModContext.Config.EnableStatusNotification.Value)
            {
                Harmony.CreateAndPatchAll(typeof(NotificationBuffPatch), PluginInfo.Guid);
                Harmony.CreateAndPatchAll(typeof(NotificationConditionPatch), PluginInfo.Guid);
                Harmony.CreateAndPatchAll(typeof(NotificationStatsPatch), PluginInfo.Guid);
            }
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

    private void Update()
    {
        ModUI.Update();
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

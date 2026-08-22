using System;
using System.Reflection;

using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.External.ModConfigGUI;
using Macchacoffee.ElinMods.ModUtility.Logging;

namespace Macchacoffee.ElinMods.EqualizeSpellitemsForMajorElements;

internal static class PluginInfo
{
    public const string Guid = "maccha-coffee.equalize-spellitems-for-major-elements";
    public const string Name = "Equalize Spellitems for Major Elements";
    public const string Version = "1.0.1";
}

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
internal class Plugin : BaseUnityPlugin
{
    private static ConfigFile? ConfigFile { get; set; }

    private void Awake()
    {
        ModLog.Initialize(Logger);
        ConfigFile = ModContext.BindConfig();
        try
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginInfo.Guid);
        }
        catch (Exception ex)
        {
            ModLog.Error($"Failed to apply harmony patch: {ex}");
        }
    }

    private void Start()
    {
        ModConfigGUISupport.ResisterConfig(PluginInfo.Guid, PluginInfo.Name, ConfigFile!);
    }
}

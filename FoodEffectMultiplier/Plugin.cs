using System;
using System.Reflection;

using BepInEx;
using BepInEx.Configuration;
using EModding;
using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Logging;

namespace Macchacoffee.ElinMods.FoodEffectMultiplier;

internal static class PluginInfo
{
    public const string Guid = "maccha-coffee.food-effect-multiplier";
    public const string Name = "Food Effect Multiplier";
    public const string Version = "1.1.0";
}

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
internal class Plugin : BaseUnityPlugin, IModConfig
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

    public void OnBuildConfig(UINote note)
    {
        note.AddAll(ConfigFile!);
    }
}

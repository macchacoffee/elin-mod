using System;
using System.Reflection;

using BepInEx;
using HarmonyLib;

using Macchacoffee.ElinMods.ModUtility.Logging;

namespace Macchacoffee.ElinMods.SimpleDamageTracker;

internal static class PluginInfo
{
    public const string Guid = "maccha-coffee.simple-damage-tracker";
    public const string Name = "Simple Damage Tracker";
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
}

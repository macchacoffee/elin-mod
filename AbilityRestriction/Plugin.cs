using System;
using System.Reflection;
using BepInEx;
using HarmonyLib;

namespace AbilityRestriction;

public static class PluginInfo
{
    public const string Guid = "maccha-coffee.ability-restriction";
    public const string Name = "Ability Restriction";
    public const string Version = "1.1.3";
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

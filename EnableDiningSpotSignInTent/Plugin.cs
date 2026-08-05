using System;
using System.Reflection;
using BepInEx;
using HarmonyLib;

namespace EnableDiningSpotSignInTent;

public static class PluginInfo
{
    public const string Guid = "maccha-coffee.enable-dining-spot-sign-in-tent";
    public const string Name = "Enable Dining Spot Sign in Tent";
    public const string Version = "1.0.2";
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

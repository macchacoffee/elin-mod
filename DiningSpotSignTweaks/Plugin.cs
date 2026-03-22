using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using HarmonyLib;

namespace DiningSpotSignTweaks;

public static class PluginInfo
{
    public const string Guid = "maccha-coffee.dining-spot-sign-tweaks";
    public const string Name = "Dining Spot Sign Tweaks";
    public const string Version = "1.0.0";
}

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
internal class Plugin : BaseUnityPlugin
{
    internal static Plugin? Instance { get; private set; }
    internal static Harmony? Harmony { get; private set; }

    private void Awake()
    {
        Instance = this;
        Harmony = new Harmony(PluginInfo.Guid);
        Harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    internal static void LogDebug(object message, [CallerMemberName] string caller = "")
    {
        Instance?.Logger.LogDebug($"[{caller}] {message}");
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

using System.IO;
using BepInEx;

namespace FoodEffectMultiplier;

public static class Mod
{
    private static readonly string ConfigFileName = $"{PluginInfo.Guid}.cfg";

    public static ModConfig Config { get; private set; } = new();
    private static string ConfigFilePath => Path.Combine(Paths.ConfigPath, ConfigFileName);

    public static void LoadConfig()
    {
        Config.Load(ConfigFilePath);
    }

    public static void SaveConfig()
    {
        Config.Save(ConfigFilePath);
    }
}

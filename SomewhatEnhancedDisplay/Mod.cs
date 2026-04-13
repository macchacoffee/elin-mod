using System.IO;
using BepInEx;
using Newtonsoft.Json;
using SomewhatEnhancedDisplay.Config;

namespace SomewhatEnhancedDisplay;

public static class Mod
{

    private static readonly string ConfigFileName = $"{PluginInfo.Guid}.txt";

    public static ModConfig Config { get; private set; } = new();

    private static string BuildConfigFilePath(string root)
    {
        return Path.Combine(root, ConfigFileName);
    }

    public static void LoadConfig(string root)
    {
        var filePath = BuildConfigFilePath(root);
        if (File.Exists(filePath))
        {
            var text = IO.IsCompressed(filePath) ? IO.Decompress(filePath) : File.ReadAllText(filePath);
            Config = ModConfig.Deserialize(text);
        }
        else
        {
            Config = new();
        }
    }

    public static void SaveConfig(string root)
    {
        var filePath = BuildConfigFilePath(root);
        var text = Config.Serialize();
        if (GameIO.compressSave)
        {
            IO.Compress(filePath, text);
        }
        else
        {
            File.WriteAllText(filePath, text);
        }
    }
}

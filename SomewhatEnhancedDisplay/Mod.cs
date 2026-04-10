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
            Config = JsonConvert.DeserializeObject<ModConfig>(text, GameIO.jsReadGame);
        }
        else
        {
            Config = new();
        }
        Plugin.LogInfo(Config.HoverGuide.Scale);
    }

    public static void SaveConfig(string root)
    {
        var filePath = BuildConfigFilePath(root);
        var text = JsonConvert.SerializeObject(Config, GameIO.formatting, GameIO.jsWriteGame);
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

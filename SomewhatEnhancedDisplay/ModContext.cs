using System.IO;
using BepInEx;
using BepInEx.Configuration;
using SomewhatEnhancedDisplay.Config;

namespace SomewhatEnhancedDisplay;

public static class ModContext
{
    private static readonly string _configFileName = $"{PluginInfo.Guid}.cfg";
    private static readonly string _worldConfigFileName = $"{PluginInfo.Guid}.txt";

    public static ModConfig Config { get; private set; } = new();
    public static ModWorldConfig WorldConfig { get; private set; } = new();

    private static string BuildWorldConfigFilePath(string root)
    {
        return Path.Combine(root, _worldConfigFileName);
    }

    public static ConfigFile BindConfig()
    {
        var configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, _configFileName), true);
        Config.Bind(configFile);
        return configFile;
    }

    public static void LoadWorldConfig(string root)
    {
        var filePath = BuildWorldConfigFilePath(root);
        if (File.Exists(filePath))
        {
            var text = IO.IsCompressed(filePath) ? IO.Decompress(filePath) : File.ReadAllText(filePath);
            WorldConfig = ModWorldConfig.Deserialize(text);
        }
        else
        {
            WorldConfig = new();
        }
    }

    public static void SaveWorldConfig(string root)
    {
        var filePath = BuildWorldConfigFilePath(root);
        var text = WorldConfig.Serialize();
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

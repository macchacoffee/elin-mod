using System.IO;
using Newtonsoft.Json;

namespace AbilityRestriction;

public static class Mod
{
    private static readonly string ConfigFileName = $"{PluginInfo.Guid}.txt";

    public static ModOriginalActStorage OriginalActStorage { get; } = new();
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

        Config.CleanUp();
    }

    public static void SaveConfig(string root)
    {
        Config.CleanUp();

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

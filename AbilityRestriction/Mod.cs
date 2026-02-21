using System.IO;
using Newtonsoft.Json;

namespace AbilityRestriction;

public static class Mod
{
    private static readonly string configFileName = $"{PluginInfo.Guid}.txt";
    public static readonly ModOriginalActStorage originalActStorage = new ModOriginalActStorage();
    public static ModConfig Config { get; private set; } = new ModConfig();

    private static string buildConfigFilePath(string root)
    {
        return Path.Combine(root, configFileName);
    }

    public static void LoadConfig(string root)
    {
        var filePath = buildConfigFilePath(root);
        if (File.Exists(filePath))
        {
            var text = IO.IsCompressed(filePath) ? IO.Decompress(filePath) : File.ReadAllText(filePath);
            Config = JsonConvert.DeserializeObject<ModConfig>(text, GameIO.jsReadGame);
        }
        else
        {
            Config = new ModConfig();
        }

        Config.CleanUp();
    }

    public static void SaveConfig(string root)
    {
        Config.CleanUp();

        var filePath = buildConfigFilePath(root);
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

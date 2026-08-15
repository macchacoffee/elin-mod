using System.IO;
using SimpleDamageTracker.Config;
using SimpleDamageTracker.Mod;

namespace SimpleDamageTracker;

internal static class ModContext
{
    private const string _worldConfigFileName = $"{PluginInfo.Guid}.txt";

    public static ModWorldConfig WorldConfig { get; private set; } = new();
    public static ModDamageTracker DamageTracker { get; } = new();

    private static string BuildWorldConfigFilePath(string root)
    {
        return Path.Combine(root, _worldConfigFileName);
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

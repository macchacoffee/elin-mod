using System.IO;

namespace AbilityRestriction;

public static class Mod
{
    private static readonly string configFileName = $"{ModInfo.Guid}.txt";
    public static readonly ModConfig config = new ModConfig();
    public static readonly ModOriginalActStorage originalActStorage = new ModOriginalActStorage();

    private static string buildConfigFilePath(string root)
    {
        return Path.Combine(root, configFileName);
    }

    public static void LoadConfig(string root)
    {
        config.Load(buildConfigFilePath(root));
    }

    public static void SaveConfig(string root)
    {
        config.Save(buildConfigFilePath(root));
    }
}
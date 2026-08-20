using BepInEx.Configuration;
using Macchacoffee.ElinMods.ModUtility.External.ModConfigGUI.UI;

namespace Macchacoffee.ElinMods.ModUtility.External.ModConfigGUI;

internal static class ModConfigGUISupport
{
    public static void ResisterConfig(string guid, string name, ConfigFile configFile)
    {
        ExternalLayerBuilder.RegisterDefaultBuilder(guid, name, configFile);
    }
}

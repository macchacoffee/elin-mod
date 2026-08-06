using BepInEx.Configuration;
using ModUtility.External.ModConfigGUI.UI;
using ModUtility.Util;

namespace ModUtility.Config;

public static class ModConfigGUISupport
{
    public static void ResisterConfig(string guid, string name, ConfigFile configFile)
    {
        ExternalLayerBuilder.RegisterDefaultBuilder(guid, name, configFile);
    }
}

using BepInEx.Configuration;
using ModUtility.Util;

namespace ModUtility.Config;

public static class ModConfigGUISupport
{
    public static void ResisterConfig(string guid, string name, ConfigFile configFile)
    {
        if (ModReflection.FindMethod(
            "ModConfigGUI", "ModConfigGUI.UI", "LayerBuilder", "RegisterDefaultBuilder",
            [typeof(string), typeof(string), typeof(ConfigFile)]) is IModMethodDelegate method)
        {
            method.Invoke(guid, name, configFile);
        }
    }
}

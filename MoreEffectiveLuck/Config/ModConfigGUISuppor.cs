using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using ModConfigGUI.UI;

namespace MoreEffectiveLuck.Config;

public static class ModConfigGUISupport
{
    public static void ResisterConfig(ConfigFile configFile)
    {
        if (GetModConfigGUIPlugin() is not null)
        {
            LayerBuilder.RegisterDefaultBuilder(PluginInfo.Guid, PluginInfo.Name, configFile);
        }
    }

    private static BaseUnityPlugin? GetModConfigGUIPlugin()
    {
        return ModManager.ListPluginObject.OfType<BaseUnityPlugin>().FirstOrDefault(plugin => plugin.Info.Metadata.GUID == "me.xtracr.modconfiggui");
    }
}

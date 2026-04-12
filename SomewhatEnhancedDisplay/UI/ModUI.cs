using BepInEx.Configuration;
using UnityEngine;

namespace SomewhatEnhancedDisplay.UI;

public static class ModUI
{
    public static void Update()
    {
        // TODO
        var key = new KeyboardShortcut(KeyCode.H);
        if (!key.IsDown())
        {
            return;
        }

        Mod.Config.HoverGuide.AdvanceStyle();
        SE.ClickGeneral();
    }
}

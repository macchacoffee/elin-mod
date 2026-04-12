using BepInEx.Configuration;
using UnityEngine;

namespace SomewhatEnhancedDisplay.UI;

public static class ModUI
{
    public static void Update()
    {
        // TODO
        var key = new KeyboardShortcut(KeyCode.H);
        if (key.IsDown())
        {
            if (EClass.ui.GetLayer<HoverGuide.ModLayerConfig>() is not null)
            {
                // Modのホバーガイド設定画面が開いている時は処理を中断する
                return;
            }

            Mod.Config.HoverGuide.AdvanceStyle();
            SE.ClickGeneral();
            return;
        }
    }
}

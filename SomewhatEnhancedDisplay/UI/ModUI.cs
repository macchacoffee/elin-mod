using BepInEx.Configuration;
using SomewhatEnhancedDisplay.UI.HoverGuide;
using SomewhatEnhancedDisplay.UI.HoverGuide.Config;
using UnityEngine;

namespace SomewhatEnhancedDisplay.UI;

public static class ModUI
{
    public static ModHoverGuide? HoverGuide { get; set; }

    public static void Update()
    {
        // TODO
        var key = new KeyboardShortcut(KeyCode.H);
        if (key.IsDown())
        {
            if (EClass.ui.GetLayer<ModLayerConfig>() is not null)
            {
                // Modのホバーガイド設定画面が開いている時は処理を中断する
                return;
            }

            Mod.Config.HoverGuide.AdvanceCurrentStyle();
            SE.ClickGeneral();
            return;
        }

        key = new KeyboardShortcut(KeyCode.L);
        if (key.IsDown())
        {
            if (EClass.ui.GetLayer<ModLayerConfig>() is not null)
            {
                // Modのホバーガイド設定画面が開いている時は処理を中断する
                return;
            }

            HoverGuide?.LocksCard = !HoverGuide?.LocksCard ?? false;
            SE.SelectHotitem();
            return;
        }
    }
}

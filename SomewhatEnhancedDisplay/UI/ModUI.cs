using BepInEx.Configuration;
using SomewhatEnhancedDisplay.UI.HoverGuide;
using SomewhatEnhancedDisplay.UI.HoverGuide.Config;

namespace SomewhatEnhancedDisplay.UI;

public static class ModUI
{
    public static ModHoverGuide? HoverGuide { get; set; }

    public static void Update()
    {
        HoverGuide?.UpdateHealthBars();

        if (new KeyboardShortcut(ModContext.Config.NextStyleKey.Value).IsDown())
        {
            if (EClass.ui.GetLayer<ModLayerConfig>() is not null)
            {
                // Modのホバーガイド設定画面が開いている時は処理を中断する
                return;
            }

            if ( ModContext.WorldConfig.HoverGuide.Styles.Count > 1)
            {
                ModContext.WorldConfig.HoverGuide.AdvanceCurrentStyle();
                SE.ClickGeneral();
            }
            return;
        }

        if (new KeyboardShortcut(ModContext.Config.LockKey.Value).IsDown())
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

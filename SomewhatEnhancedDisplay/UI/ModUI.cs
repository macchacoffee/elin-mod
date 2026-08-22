using BepInEx.Configuration;

using Macchacoffee.ElinMods.SomewhatEnhancedDisplay.UI.HoverGuide.Config;

namespace Macchacoffee.ElinMods.SomewhatEnhancedDisplay.UI;

internal static class ModUI
{
    public static HoverGuide.HoverGuide? HoverGuide { get; set; }

    public static void Update()
    {
        HoverGuide?.UpdateHealthBars();

        if (new KeyboardShortcut(ModContext.Config.NextStyleKey.Value).IsDown())
        {
            if (EClass.ui.GetLayer<LayerModConfig>() is not null)
            {
                // Modのホバーガイド設定画面が開いている時は処理を中断する。
                return;
            }

            if (ModContext.WorldConfig.HoverGuide.Styles.Count > 1)
            {
                ModContext.WorldConfig.HoverGuide.AdvanceCurrentStyle();
                SE.ClickGeneral();
            }
            return;
        }

        if (new KeyboardShortcut(ModContext.Config.LockKey.Value).IsDown())
        {
            if (EClass.ui.GetLayer<LayerModConfig>() is not null)
            {
                // Modのホバーガイド設定画面が開いている時は処理を中断する。
                return;
            }

            if (HoverGuide?.TryToggleLock() == true)
            {
                SE.SelectHotitem();
            }
            return;
        }
    }
}

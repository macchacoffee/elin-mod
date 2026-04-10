using System;

namespace SomewhatEnhancedDisplay.UI;

public static class ModUIUtil
{
    public static int ComputeFontSize(int baseSize)
    {
        var gameConfigSize = EClass.core.config.font.fontWidget.size;
        return (int)((baseSize + gameConfigSize) * Mod.Config.HoverGuide.Scale);
    }

    public static float GetHealthRatio(Chara chara)
    {
        var health = Math.Max(chara.hp, 0);
        var maxHealth = Math.Max(chara.MaxHP, 0);
        if (chara.HasElement(FEAT.featManaMeat))
        {
            // マナの体フィートを持っている場合はマナも体力の一部として扱う
            health += Math.Max(chara.mana.value, 0);
            maxHealth += Math.Max(chara.mana.max, 0);
        }
        return (float)health / maxHealth;
    }
}
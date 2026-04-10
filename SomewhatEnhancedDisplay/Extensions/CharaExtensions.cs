using System;

namespace SomewhatEnhancedDisplay.Extensions;

public static class CharaExtensions
{
    extension(Chara chara)
    {
        public float HealthRatio
        {
            get
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

        public bool IsInFullHealth => chara.HealthRatio >= 1;
    }
}
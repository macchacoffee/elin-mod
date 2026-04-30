using System;

namespace SomewhatEnhancedDisplay.Extensions;

public static class CharaExtensions
{
    extension(Chara chara)
    {
        public double HealthRatio
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
                return (double)health / maxHealth;
            }
        }

        public bool IsInFullHealth => chara.HealthRatio >= 1;

        public bool IsElite => chara.IsPowerful;

        public bool IsBoss => chara.uid == EClass._zone.uidBoss;

        public Chara? MimicryChara => chara.mimicry is ConTransmuteHuman trans ? trans.chara : null;

        public Thing? MimicryThing => chara.mimicry is ConTransmuteMimic trans ? trans.thing : null;

        public Chara MimicryOrSelf => chara.MimicryChara is Chara c ? c : chara;

        public bool HasMimicry => chara.mimicry is not null;

        public bool HasMimicryChara => chara.MimicryChara is not null;

        public bool HasMimicryThing => chara.MimicryThing is not null;
    }
}
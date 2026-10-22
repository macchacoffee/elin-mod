using System;

namespace Macchacoffee.ElinMods.AdditionalUniqueNPC.Mod;

internal static class ManaAbsorption
{
    [ThreadStatic]
    private static bool _isMagicDamage;

    public static bool IsMagicSource(ActRef actRef)
    {
        if (actRef.refThing != null)
        {
            return actRef.refThing.trait is TraitRod or TraitScroll;
        }

        return actRef.act is Spell or ActBreathe or Song;
    }

    public static void OnDamageApplied(Card target, long dmg)
    {
        var isMagic = _isMagicDamage;
        _isMagicDamage = false;

        if (!isMagic ||
            dmg <= 0 ||
            target.Chara is not Chara chara ||
            target.hp < 0 ||
            chara.isDead)
        {
            return;
        }

        var level = target.Evalue(ModConsts.SourceNumericId.FeatManaAbsorber);
        if (level <= 0)
        {
            return;
        }

        var mana = chara.mana;
        var amount = GetRecovery(dmg, level, mana.value, mana.max);
        if (amount > 0)
        {
            mana.Mod(amount);
        }
    }

    private static int GetRecovery(long dmg, int level, int currentMana, int maxMana)
    {
        if (dmg <= 0 || level <= 0)
        {
            return 0;
        }

        var capacity = Math.Max(0L, (long)maxMana - currentMana);
        var recovery = dmg > long.MaxValue / level
            ? capacity
            : Math.Min(capacity, dmg * level / 10);
        return (int)Math.Min(recovery, int.MaxValue);
    }

    public static void DamageHP(
        Card target,
        long dmg,
        int ele,
        int eleP,
        AttackSource attackSource,
        Card? origin,
        bool showEffect,
        Thing? weapon,
        Chara? originalTarget,
        int resistPenetrationLevel,
        bool isMagic)
    {
        var previous = _isMagicDamage;
        _isMagicDamage = isMagic;

        try
        {
            target.DamageHP(
                dmg,
                ele,
                eleP,
                attackSource,
                origin,
                showEffect,
                weapon,
                originalTarget,
                resistPenetrationLevel
            );
        }
        finally
        {
            _isMagicDamage = previous;
        }
    }

    public static void DamageHP(
        Card target,
        long dmg,
        AttackSource attackSource,
        Card? origin,
        bool isMagic)
    {
        var previous = _isMagicDamage;
        _isMagicDamage = isMagic;

        try
        {
            target.DamageHP(dmg, attackSource, origin);
        }
        finally
        {
            _isMagicDamage = previous;
        }
    }
}

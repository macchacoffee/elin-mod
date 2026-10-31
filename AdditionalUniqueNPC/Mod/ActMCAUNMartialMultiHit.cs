using System.Collections.Generic;

namespace Macchacoffee.ElinMods.AdditionalUniqueNPC.Mod;

public class ActMCAUNMartialMultiHit : Ability
{
    public override Cost GetCost(Chara c)
    {
        var result = base.GetCost(c);

        if (result.type != CostType.SP)
        {
            return result;
        }

        var cost = source.cost[0] + (c.body.CountWeapons() - 2) * 2;

        if (!c.IsPC && cost > 2)
        {
            cost /= 2;
        }

        result.cost = cost;
        return result;
    }

    public override bool CanPerform()
    {
        if (TC == null || !ACT.Melee.CanPerform())
        {
            return false;
        }

        return MartialMeleeAbility.CanPerformWithEquipedWeapons(CC);
    }

    public override bool Perform()
    {
        // エフェクトと効果音のために装備中の対象武器を収集する。
        var weapons = new List<Thing>();

        foreach (var slot in CC.body.slots)
        {
            var weapon = slot.thing;

            if (slot.elementId == SLOT.hand &&
                weapon != null &&
                weapon.IsMeleeWeapon &&
                weapon.category.skill == SKILL.martial &&
                weapon.source.offense.Length >= 2)
            {
                weapons.Add(weapon);
            }
        }

        var count = 3 + rnd(4) + CC.Evalue(FEAT.featComat);

        for (var i = 0; i < count; i++)
        {
            if (!CC.IsAliveInCurrentZone ||
                !TC.IsAliveInCurrentZone)
            {
                break;
            }

            // エフェクトと効果音は対象武器からランダムに選択する。
            var weapon = weapons.Count > 0
                ? weapons[rnd(weapons.Count)]
                : null;

            var effectId = GetHitEffectId(CC, weapon);
            var playEffect = i % 3 == 0;
            var playHitSound = i % 2 == 0;
            var delay = i * 0.07f;

            TweenUtil.Delay(delay, delegate
            {
                if (playEffect)
                {
                    TC.pos.PlayEffect("scream");
                    TC.pos.PlayEffect(effectId);
                }
                if (playHitSound)
                {
                    TC.pos.PlaySound("mc_aun_martial_hit1");
                }
                TC.pos.PlaySound(effectId);
            });

            new ActMCAUNMartialMeleeStorm().Perform(CC, TC);
        }

        return true;
    }

    private static string GetHitEffectId(Chara attacker, Thing? weapon)
    {
        var attackType = !string.IsNullOrEmpty(attacker.race.meleeStyle)
            ? attacker.race.meleeStyle.ToEnum<AttackType>()
            : AttackType.Punch;

        if (weapon != null &&
            !string.IsNullOrEmpty(weapon.source.attackType))
        {
            attackType = weapon.source.attackType.ToEnum<AttackType>();
        }

        return attackType switch
        {
            AttackType.Slash => "hit_slash",
            AttackType.Spore => "hit_spore",
            AttackType.Claw or AttackType.Bite => "hit_claw",
            AttackType.Blunt or AttackType.Punch or AttackType.Kick or
                AttackType.Bow or AttackType.Gun or AttackType.Cane => "hit_blunt",
            _ => "hit_default",
        };
    }
}

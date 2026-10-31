using System;
using System.Collections.Generic;
using System.Linq;

namespace Macchacoffee.ElinMods.AdditionalUniqueNPC.Mod;

public class ActMCAUNMartialSwarm : Ability
{
    public override bool CanPerform()
    {
        if (TC == null ||
            CC == TC ||
            !ACT.Melee.CanPerform())
        {
            return false;
        }

        return MartialMeleeAbility.CanPerformWithEquipedWeapons(CC);
    }

    public override bool Perform()
    {
        var delay = 0f;
        var hashSet = new HashSet<int>();

        foreach (var card in _map.Cards.ToList())
        {
            if (!CC.IsAliveInCurrentZone)
            {
                break;
            }

            if (!card.IsAliveInCurrentZone ||
                card == CC ||
                (card.isChara && card != TC && !card.Chara.IsHostile(CC)) ||
                (!card.isChara && !card.trait.CanBeAttacked) ||
                card.Dist(CC) > ACT.Melee.PerformDistance ||
                !CC.CanSeeLos(card))
            {
                continue;
            }

            var pos = card.pos;

            if (!hashSet.Contains(pos.index))
            {
                hashSet.Add(pos.index);

                TweenUtil.Delay(delay, delegate
                {
                    TC.pos.PlayEffect("scream");
                    TC.pos.PlaySound("mc_aun_martial_hit1");
                });

                if (delay < 1f)
                {
                    delay += 0.07f;
                }

                new ActMCAUNMartialMeleeSwarm().Perform(CC, card);
            }
        }

        return true;
    }
}

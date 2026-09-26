namespace Macchacoffee.ElinMods.AlliesSwitchRangedWeapons.Mod;

internal static class RangedWeaponSwitcher
{
    public static void TrySwitchRangedWeapon(Chara? chara, Card? target)
    {
        if (chara is null || !IsTarget(chara) || chara.id == "mamani"
            || chara.isRestrained || chara.HasCondition<ConReload>())
        {
            return;
        }

        var candidateCount = 0;
        foreach (var weapon in chara.things)
        {
            if (IsCandidate(chara, weapon, target))
            {
                candidateCount++;
            }
        }

        if (candidateCount < 2)
        {
            return;
        }

        var candidateIndex = EClass.rnd(candidateCount);
        foreach (var weapon in chara.things)
        {
            if (!IsCandidate(chara, weapon, target))
            {
                continue;
            }

            if (candidateIndex == 0)
            {
                chara.ranged = weapon;
                return;
            }
            candidateIndex--;
        }
    }

    private static bool IsTarget(Chara chara)
    {
        return !chara.IsPC && chara.hostility == Hostility.Ally;
    }

    private static bool IsCandidate(Chara chara, Thing weapon, Card? target)
    {
        return weapon.parent == chara && weapon.IsRangedWeapon
            && chara.CanEquipRanged(weapon) && weapon.CanAutoFire(chara, target);
    }
}

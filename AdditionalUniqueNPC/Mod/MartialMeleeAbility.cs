namespace Macchacoffee.ElinMods.AdditionalUniqueNPC.Mod;

public static class MartialMeleeAbility
{
    public static bool CanPerformWithEquipedWeapons(Chara? chara)
    {
        if (chara == null)
        {
            return false;
        }

        foreach (var slot in chara.body.slots)
        {
            var weapon = slot.thing;

            // 素手か暗器を装備している場合のみ使用可能。
            if (weapon != null && weapon.IsWeapon &&
                (!weapon.IsMeleeWeapon || weapon.category.skill != SKILL.martial))
            {
                return false;
            }
        }

        return true;
    }
}

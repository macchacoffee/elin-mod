using UnityEngine;

namespace Macchacoffee.ElinMods.AdditionalUniqueNPC.Mod;

public class ActMCAUNMartialCounter : Ability
{
    public override bool CanPerform()
    {
        return MartialMeleeAbility.CanPerformWithEquipedWeapons(CC) &&
            !CC.HasCooldown(id) &&
            !CC.HasCondition<ConMCAUNMartialCounter>();
    }

    public override bool Perform()
    {
        if (CC.HasCooldown(id) || CC.HasCondition<ConMCAUNMartialCounter>())
        {
            return false;
        }

        var power = Mathf.Max(1, CC.Evalue(SKILL.martial));
        return CC.AddCondition<ConMCAUNMartialCounter>(power) != null;
    }
}

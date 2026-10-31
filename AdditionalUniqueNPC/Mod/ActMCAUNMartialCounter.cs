namespace Macchacoffee.ElinMods.AdditionalUniqueNPC.Mod;

public class ActMCAUNMartialCounter : Ability
{
    public override bool CanPerform()
    {
        return MartialMeleeAbility.CanPerformWithEquipedWeapons(CC) &&
            !CC.HasCondition<ConMCAUNMartialCounter>();
    }

    public override bool Perform()
    {
        if (CC.HasCondition<ConMCAUNMartialCounter>())
        {
            return false;
        }

        return CC.AddCondition<ConMCAUNMartialCounter>() != null;
    }
}

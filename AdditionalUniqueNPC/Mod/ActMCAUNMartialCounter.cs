namespace Macchacoffee.ElinMods.AdditionalUniqueNPC.Mod;

public class ActMCAUNMartialCounter : Ability
{
    public override bool CanPerform()
    {
        return MartialMeleeAbility.CanPerformWithEquipedWeapons(CC);
    }

    public override bool Perform()
    {
        CC.AddCondition<ConMCAUNMartialCounter>();
        return true;
    }
}

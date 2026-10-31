namespace Macchacoffee.ElinMods.AdditionalUniqueNPC.Mod;

public class ActMCAUNMartialMeleeSwarm : ActMelee
{
    public override float BaseDmgMTP => 0.9f + 0.1f * CC.Evalue(1750);
}

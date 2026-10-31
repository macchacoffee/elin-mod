namespace Macchacoffee.ElinMods.AdditionalUniqueNPC.Mod;

internal class ConMCAUNMartialCounter : BaseBuff
{
    public override bool CanStack(Condition c) => false;

    public override void Tick()
    {
    }

    public static void OnActPerformed(Act act)
    {
        if (act is ActMeleeCounter)
        {
            Act.CC.GetCondition<ConMCAUNMartialCounter>()?.Mod(-1);
        }
    }
}

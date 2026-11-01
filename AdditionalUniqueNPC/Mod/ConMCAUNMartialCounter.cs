namespace Macchacoffee.ElinMods.AdditionalUniqueNPC.Mod;

internal class ConMCAUNMartialCounter : BaseBuff
{
    public override bool CanStack(Condition c) => false;

    public override int EvaluateTurn(int p)
    {
        return base.EvaluateTurn(p) + EClass.curve(p, 50, 25, 75) / 20;
    }

    public override void Tick()
    {
    }

    public override void OnRemoved()
    {
        owner.SetCooldown(ModConsts.SourceNumericId.ActMartialCounter, 20);
    }

    public static void OnActPerformed(Act act)
    {
        if (act is ActMeleeCounter)
        {
            Act.CC.GetCondition<ConMCAUNMartialCounter>()?.Mod(-1);
        }
    }
}

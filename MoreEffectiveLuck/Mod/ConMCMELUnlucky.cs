namespace MoreEffectiveLuck.Mod;

internal class ConMCMELUnlucky : BaseDebuff
{
    public override int EvaluateTurn(int p)
    {
        return rnd(500) + 500;
    }
}

namespace MoreEffectiveLuck.Mod;

public class ConMCMELUnlucky : BaseDebuff
{
    public override int EvaluateTurn(int p)
    {
        return rnd(500) + 500;
    }
}

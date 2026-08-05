namespace MoreEffectiveLuck.Mod;

public class ConMCMELLucky : BaseBuff
{
    public override int EvaluateTurn(int p)
    {
        return rnd(500) + 500;
    }
}

namespace Macchacoffee.ElinMods.MoreEffectiveLuck.Mod;

internal class ConMCMELLucky : BaseBuff
{
    public override int EvaluateTurn(int p)
    {
        return rnd(500) + 500;
    }
}

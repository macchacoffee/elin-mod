namespace MoreEffectiveLuck.Game;

public class ConMCMELLuckyDay : Condition
{
    public override bool CanManualRemove => true;

    public override int GetPhase()
    {
        return 0;
    }

    public override bool CanStack(Condition c)
    {
        return c.power >= power;
    }
}

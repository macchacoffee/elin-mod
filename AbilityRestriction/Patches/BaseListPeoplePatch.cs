namespace AbilityRestriction.Patches;

public static class BaseListPeoplePatch
{
    public static Chara? TargetChara { get; set; }

    public static void OnClick_Prefix(BaseListPeople __instance, Chara c, ItemGeneral i)
    {
        if (__instance.GetType() != typeof(ListPeople))
        {
            return;
        }
        if (c.IsPC || !c.IsHomeMember())
        {
            return;
        }

        TargetChara = c;
    }
}

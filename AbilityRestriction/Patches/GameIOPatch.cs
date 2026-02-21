namespace AbilityRestriction.Patches;

public static class GameIOPatch
{
    public static void SaveGame_Prefix()
    {
        Mod.SaveConfig(GameIO.pathCurrentSave);
    }
}

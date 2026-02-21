namespace AbilityRestriction.Patches;

public static class GamePatch
{
    public static void Load_Postfix(string id, bool cloud)
    {
        var root = cloud ? CorePath.RootSaveCloud : CorePath.RootSave + id;
        Mod.LoadConfig(root);
    }
}

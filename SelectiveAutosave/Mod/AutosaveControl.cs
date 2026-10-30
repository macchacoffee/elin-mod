namespace Macchacoffee.ElinMods.SelectiveAutosave.Mod;

internal static class AutosaveControl
{
    public static bool SaveOnMapChange(Game game, bool isAutoSave, bool silent)
    {
        return !ModContext.Config.SaveOnMapChange.Value || game.Save(isAutoSave, silent);
    }

    public static bool SaveOnSleep(Game game, bool isAutoSave, bool silent)
    {
        return !ModContext.Config.SaveOnSleep.Value || game.Save(isAutoSave, silent);
    }

    public static bool SaveOnRelocation(Game game, bool isAutoSave, bool silent)
    {
        return !ModContext.Config.SaveOnRelocation.Value || game.Save(isAutoSave, silent);
    }

    public static bool SaveOnEvent(Game game, bool isAutoSave, bool silent)
    {
        return !ModContext.Config.SaveOnEvent.Value || game.Save(isAutoSave, silent);
    }

    public static void RequestBossAutosave(Player player, bool value)
    {
        if (ModContext.Config.SaveOnBossDefeat.Value)
        {
            player.willAutoSave = value;
        }
    }

    public static void RequestTreasureAutosave(Player player, bool value)
    {
        if (ModContext.Config.SaveOnTreasureDiscovery.Value)
        {
            player.willAutoSave = value;
        }
    }
}

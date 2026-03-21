using BepInEx.Configuration;

namespace FoodEffectMultiplier;

public class ModConfig
{
    private static readonly string General = "General";
    private enum FileSyncType
    {
        Load,
        Save,
    }

    public float? PCMultiplier { get; private set; } = null;
    public float? NPCMultiplier { get; private set; } = null;

    private void SyncFile(string filePath, FileSyncType syncType)
    {
        var file = new ConfigFile(filePath, true);

        var pcMultiplier = file.Bind(General, "PCMultiplier", -1f, new ConfigDescription(
            "PCの食事効果倍率 (負の値の場合はゲームのデフォルト値を使用 (1倍))\nPC Food Effect Mutiplier (negative value means game default multiplier (1x))",
            new AcceptableValueRange<float>(-1f, 1000f)
        ));
        var npcMultiplier = file.Bind(General, "NPCMultiplier", -1f, new ConfigDescription(
            "NPCの食事効果倍率 (負の値の場合はゲームのデフォルト値を使用 (3倍))\nNPC Food Effect Mutiplier (negative value means game default multiplier (3x))",
            new AcceptableValueRange<float>(-1f, 1000f)
        ));

        file.SaveOnConfigSet = false;

        switch (syncType)
        {
            case FileSyncType.Load:
                PCMultiplier = pcMultiplier.Value >= 0.0f ? pcMultiplier.Value : null;
                NPCMultiplier = npcMultiplier.Value >= 0.0f ? npcMultiplier.Value : null;
                break;
            case FileSyncType.Save:
                pcMultiplier.Value = PCMultiplier ?? -1f;
                npcMultiplier.Value = NPCMultiplier ?? -1f;
                break;
        }

        if (syncType == FileSyncType.Save)
        {
            file.Save();
        }
    }

    public void Load(string filePath)
    {
        SyncFile(filePath, FileSyncType.Load);
    }

    public void Save(string filePath)
    {
        SyncFile(filePath, FileSyncType.Save);
    }
}

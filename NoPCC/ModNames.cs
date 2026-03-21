using ModUtility.Resource;

namespace NoPCC;

public static class ModNames
{
    public static readonly ModName NoPCC = new(
        "No PCC",
        "No PCC",
        "No PCC"
    );

    public static readonly ModName Enable = new(
        "有効",
        "Enable",
        "启用"
    );

    public static readonly ModName GeneralSettings = new(
        "全体設定",
        "General",
        "全局设置"
    );

    public static readonly ModName EnableMod = new(
        "PCCをスプライトに置き換える",
        "Replace PCC to sprite",
        "将PCC替换为图片"
    );

    public static readonly ModName SpriteSettings = new(
        "スプライト設定",
        "Sprite",
        "图片设置"
    );

    public static readonly ModName DefaultSprite = new(
        "デフォルト",
        "Default",
        "默认"
    );

    public static readonly ModName SnowSprite = new(
        "冬服",
        "Winter",
        "冬装"
    );

    public static readonly ModName UndressSprite = new(
        "脱衣中",
        "Undressing",
        "脱衣中"
    );

    public static readonly ModName RideSprite = new(
        "騎乗中",
        "Riding",
        "骑乘中"
    );

    public static readonly ModName RideSnowSprite = new(
        "騎乗中 (冬服)",
        "Riding (Winter)",
        "骑乘中 (冬装)"
    );

    public static readonly ModName CombatSprite = new(
        "戦闘時",
        "Combat",
        "战斗中"
    );

    public static readonly ModName CombatSnowSprite = new(
        "戦闘時 (冬服)",
        "Combat (Winter)",
        "战斗中 (冬装)"
    );

    public static readonly ModName RideCombatSprite = new(
        "騎乗中戦闘時",
        "Riding Combat",
        "骑乘战斗中"
    );

    public static readonly ModName RideCombatSnowSprite = new(
        "騎乗中戦闘時 (冬服)",
        "Riding Combat (Winter)",
        "骑乘战斗中 (冬装)"
    );
}

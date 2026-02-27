using ModUtility.Resource;

namespace AbilityRestriction;

public static class ModNames
{
    public static readonly ModName ModName = new(
        TextJp: "Ability Restriction",
        TextEn: "Ability Restriction",
        TextCn: "Ability Restriction"
    );

    public static readonly ModName RestrictAbilities = new(
        TextJp: "アビリティの使用制限",
        TextEn: "Restrict Abilities",
        TextCn: "能力使用限制"
    );

    public static readonly ModName Party = new(
        TextJp: "全体",
        TextEn: "Party",
        TextCn: "全部"
    );
}

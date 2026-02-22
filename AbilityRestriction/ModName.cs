namespace AbilityRestriction;

public class ModName
{
    private readonly string TextJp;
    private readonly string TextEn;
    private readonly string TextCn;

    public string Text
    {
        get
        {
            switch (Lang.langCode)
            {
                case "JP":
                    return TextJp;
                case "EN":
                    return TextEn;
                case "CN":
                    return TextCn;
            }
            return TextJp;
        }
    }

    public ModName(string textJp, string textEn, string textCn)
    {
        TextJp = textJp;
        TextEn = textEn;
        TextCn = textCn;
    }
}

public static class ModNames
{
    public static readonly ModName ModName = new("Ability Restriction", "Ability Restriction", "Ability Restriction");
    public static readonly ModName RestrictAbilities = new("アビリティの使用制限", "Restrict Abilities", "能力使用限制");
    public static readonly ModName Party = new("全体", "Party", "全部");
}

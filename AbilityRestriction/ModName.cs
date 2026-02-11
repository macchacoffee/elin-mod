namespace AbilityRestriction;

public class ModName
{
    private readonly string textJp;
    private readonly string textEn;
    private readonly string textCn;

    public string Text
    {
        get
        {
            switch (Lang.langCode)
            {
                case "JP":
                    return textJp;
                case "EN":
                    return textEn;
                case "CN":
                    return textCn;
            }
            return textJp;
        }
    }

    public ModName(string textJp, string textEn, string textCn)
    {
        this.textJp = textJp;
        this.textEn = textEn;
        this.textCn = textCn;
    }
}

public static class ModNames
{
    public static readonly ModName modName = new ModName("Ability Restriction", "Ability Restriction", "Ability Restriction");

    public static readonly ModName restrictAbilities = new ModName("アビリティの使用制限", "Restrict Abilities", "能力使用限制");
}

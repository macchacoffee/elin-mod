namespace ModUtility.Resource;

public record ModName(string TextJp, string TextEn, string TextCn)
{
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
}

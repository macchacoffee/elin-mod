namespace ModUtility.Resource;

/// <summary>多言語対応文字列</summary>
/// <param name="TextJp">日本語文字列</param>
/// <param name="TextEn">英語文字列</param>
/// <param name="TextCn">中国語文字列</param>
public record ModName(string TextJp, string TextEn, string TextCn)
{
    /// <value>ゲームの言語設定に基づく文字列</value>
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

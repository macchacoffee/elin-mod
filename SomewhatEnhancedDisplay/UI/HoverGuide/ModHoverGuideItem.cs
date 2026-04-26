using SomewhatEnhancedDisplay.Config;
using SomewhatEnhancedDisplay.Extensions;

namespace SomewhatEnhancedDisplay.UI.HoverGuide;

public class ModHoverGuideItem
{
    private static readonly float PaddingHeight = 3;

    private UIText TextName1 { get; }
    private ModHealthBar HealthBar { get; }
    private ModHoverGuidePadding Padding { get; }
    private UIText TextName2 { get; }

    public bool Enabled
    {
        get
        {
            return
                TextName1.enabled
                || HealthBar.Enabled
                || Padding.Enabled
                || TextName2.enabled;
        }
        set
        {
            TextName1.enabled = value;
            HealthBar.Enabled = value;
            Padding.Enabled = value;
            TextName2.enabled = value;
        }
    }

    private static ModConfigHoverGuide Config => Mod.Config.HoverGuide;
    private static ModConfigHoverGuideColorSet ColorConfig => Config.ColorSet;
    private static ModConfigHoverGuideStyleChara StyleConfig => Config.CurrentStyle.Chara;

    public ModHoverGuideItem(WidgetMouseover widget) : this(widget, false) { }

    public ModHoverGuideItem(WidgetMouseover widget, bool prepends)
    {
        var localScale = widget.textName.transform.localScale;

        TextName1 = UnityEngine.Object.Instantiate(widget.textName);
        TextName1.transform.SetParent(widget.layout.transform);
        TextName1.transform.localScale = localScale;

        HealthBar = new(widget);

        Padding = new(widget);

        TextName2 = UnityEngine.Object.Instantiate(widget.textName);
        TextName2.transform.SetParent(widget.layout.transform);
        TextName2.transform.localScale = localScale;

        if (prepends)
        {
            // 先頭 (textNameの前) に追加する
            var index = widget.textName.transform.GetSiblingIndex();
            TextName2.transform.SetSiblingIndex(index);
            Padding.Image.transform.SetSiblingIndex(index);
            HealthBar.Layout.transform.SetSiblingIndex(index);
            TextName1.transform.SetSiblingIndex(index);
        }

        // ウィジェットを無効から有効に切り替えた際に表示が乱れないようにするため、
        // 初期状態では追加コンポーネントなどは表示しないようにする
        TextName1.enabled = false;
        HealthBar.Enabled = false;
        Padding.Enabled = false;
        TextName2.enabled = false;
    }

    public bool Show(FontColor fontColor, int fontSize, int baseFontSize, ModHoverGuideTarget? target)
    {
        var paddingHeight = PaddingHeight * fontSize / baseFontSize;

        var displays = false;
        var isPaddingRequired = false;
        if (target?.Text1 is string text1 && !text1.IsEmpty())
        {
            text1 = text1.TagColorNullable(ColorConfig.MainTextColor);
            TextName1.fontColor = fontColor;
            TextName1.fontSize = fontSize;
            TextName1.text = text1.TagSize(fontSize);
            TextName1.enabled = true;
            displays = true;
            isPaddingRequired = true;
        }
        else
        {
            TextName1.text = string.Empty;
            TextName1.enabled = false;
        }
        if (target?.Card is Chara chara && (!StyleConfig.EnableMimicry || !chara.HasMimicryThing))
        {
            var enabled = DisplaysHealthBar(chara);
            HealthBar.Enabled = enabled;
            HealthBar.Update(chara);
            displays = enabled;
            isPaddingRequired = !enabled;
        }
        else
        {
            HealthBar.Enabled = false;
        }
        if (target?.Text2 is string text2 && !text2.IsEmpty())
        {
            text2 = text2.TagColorNullable(ColorConfig.MainTextColor);
            TextName2.fontColor = fontColor;
            TextName2.fontSize = fontSize;
            TextName2.text = text2.TagSize(fontSize);
            TextName2.enabled = true;
            displays = true;
        }
        else
        {
            TextName2.text = string.Empty;
            TextName2.enabled = false;
            isPaddingRequired = false;
        }

        Padding.Update(isPaddingRequired, paddingHeight);

        return displays;
    }

    public void ShowForManager()
    {
        TextName1.enabled = false;
        HealthBar.Enabled = false;
        Padding.Enabled = false;
        TextName2.enabled = false;
    }

    private bool DisplaysHealthBar(Chara chara)
    {
        if (!StyleConfig.DisplayHealthBar)
        {
            return false;
        }

        var config = StyleConfig.HealthBar;
        switch (chara.hostility)
        {
            case Hostility.Enemy:
                return DisplaysHealthBar(chara, config.DisplayForEnemy);
            case Hostility.Neutral:
                return DisplaysHealthBar(chara, config.DisplayForNetural);
            case Hostility.Friend:
                return DisplaysHealthBar(chara, config.DisplayForFriend);
            case Hostility.Ally:
                return DisplaysHealthBar(chara, config.DisplayForAlly);
            default:
                return false;
        }
    }

    private bool DisplaysHealthBar(Chara chara, ModConfigHealthBarDisplay config)
    {
        var displays = config.Target switch
        {
            ModHealthBarDisplayTarget.None => false,
            ModHealthBarDisplayTarget.Boss => chara.IsBoss,
            ModHealthBarDisplayTarget.Elite => chara.IsElite,
            ModHealthBarDisplayTarget.All => true,
            _ => false,
        };
        if (!displays)
        {
            return false;
        }

        if (!config.NotInCombat && !chara.IsInCombat)
        {
            return false;
        }

        return config.InFullHealth || !chara.IsInFullHealth;
    }
}

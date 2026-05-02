using System;
using SomewhatEnhancedDisplay.Config;
using SomewhatEnhancedDisplay.Extensions;

namespace SomewhatEnhancedDisplay.UI.HoverGuide;

public class ModHoverGuideItem
{
    private static readonly int PaddingHeight = 1;

    private UIText TextName1 { get; }
    private ModHealthBar HealthBar { get; }
    private UIText TextName2 { get; }

    public bool Enabled
    {
        get
        {
            return
                TextName1.enabled
                || HealthBar.Enabled
                || TextName2.enabled;
        }
        set
        {
            TextName1.enabled = value;
            HealthBar.Enabled = value;
            TextName2.enabled = value;
        }
    }

    private static ModConfigHoverGuide Config => Mod.Config.HoverGuide;
    private static ModConfigHoverGuideColorSet ColorConfig => Config.ColorSet;
    private static ModConfigHoverGuideStyleChara StyleConfig => Config.CurrentStyle.Chara;

    public ModHoverGuideItem(WidgetMouseover widget)
    {
        var localScale = widget.textName.transform.localScale;

        TextName1 = UnityEngine.Object.Instantiate(widget.textName);
        TextName1.name = ModConsts.GameObjectName.HoverGuideText;
        TextName1.transform.SetParent(widget.layout.transform);
        TextName1.transform.localScale = localScale;

        HealthBar = new(widget);

        TextName2 = UnityEngine.Object.Instantiate(widget.textName);
        TextName2.name = ModConsts.GameObjectName.HoverGuideText;
        TextName2.transform.SetParent(widget.layout.transform);
        TextName2.transform.localScale = localScale;

        // ウィジェットを無効から有効に切り替えた際に表示が乱れないようにするため、
        // 初期状態では追加コンポーネントなどは表示しないようにする
        TextName1.enabled = false;
        HealthBar.Enabled = false;
        TextName2.enabled = false;
    }

    public bool Show(FontColor fontColor, int fontSize, ModHoverGuideTarget? target, bool isLocked)
    {
        var displays = false;
        var isPaddingRequired = false;
        if (target?.Text1 is string text1 && !text1.IsEmpty())
        {
            if (isLocked)
            {
                var lines = text1.Split([Environment.NewLine], StringSplitOptions.None);
                if (lines.Length > 0)
                {
                    lines[0] = $"* {lines[0]} *";
                }
                text1 = string.Join(Environment.NewLine, lines);
            }
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
            HealthBar.Update(chara, target.Modifier);
            displays = HealthBar.Enabled;
            isPaddingRequired = !displays;
        }
        else
        {
            HealthBar.Enabled = false;
        }
        if (target?.Text2 is string text2 && !text2.IsEmpty())
        {
            if (isPaddingRequired)
            {
                text2 = $"{Environment.NewLine.TagSize(ModUIUtil.ComputeFontSize(PaddingHeight))}{text2}";
            }
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
        }

        return displays;
    }

    public void ShowForManager()
    {
        TextName1.enabled = false;
        HealthBar.Enabled = false;
        TextName2.enabled = false;
    }

    public void ClearTarget()
    {
        HealthBar.ClearTarget();
    }
}

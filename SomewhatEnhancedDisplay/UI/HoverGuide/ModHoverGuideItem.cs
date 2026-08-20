using System;

using Macchacoffee.ElinMods.SomewhatEnhancedDisplay.Config;
using Macchacoffee.ElinMods.SomewhatEnhancedDisplay.Extensions;

namespace Macchacoffee.ElinMods.SomewhatEnhancedDisplay.UI.HoverGuide;

internal class ModHoverGuideItem
{
    private const int _paddingHeight = 1;

    private UIText TextName1 { get; }
    private ModHealthBar HealthBar { get; }
    private UIText TextName2 { get; }

    public bool Enabled => TextName1.enabled || HealthBar.Enabled || TextName2.enabled;

    private static ModConfigHoverGuide Config => ModContext.WorldConfig.HoverGuide;
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
        // 初期状態では追加コンポーネントなどは表示しないようにする。
        TextName1.enabled = false;
        HealthBar.Hide();
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
                text1 = AddLockMarker(text1);
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
        if (target?.Card is Chara chara && (StyleConfig.DisableMimicry || !chara.HasMimicryThing))
        {
            HealthBar.UpdateTarget(chara, target.Modifier);
            displays = HealthBar.Enabled;
            isPaddingRequired = !displays;
        }
        else
        {
            HealthBar.Hide();
            if (target?.Card is not null)
            {
                HealthBar.StopTracking();
            }
        }
        if (target?.Text2 is string text2 && !text2.IsEmpty())
        {
            if (isPaddingRequired)
            {
                text2 = $"{Environment.NewLine.TagSize(ModUIUtil.ComputeFontSize(_paddingHeight))}{text2}";
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
        HealthBar.Hide();
        HealthBar.StopTracking();
        TextName2.enabled = false;
    }

    public void UpdateHealthBar()
    {
        HealthBar.Update();
    }

    public void ClearTarget()
    {
        HealthBar.StopTracking();
    }

    private static string AddLockMarker(string text)
    {
        var index = text.IndexOf(Environment.NewLine);
        if (index < 0)
        {
            return $"* {text} *";
        }

        return $"* {text[..index]} *{text[index..]}";
    }
}

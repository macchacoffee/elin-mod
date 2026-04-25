using System;
using SomewhatEnhancedDisplay.Config;
using SomewhatEnhancedDisplay.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace SomewhatEnhancedDisplay.UI.HoverGuide;

public class ModHoverGuide
{
    private static readonly float PaddingHeight = 3;

    private ModHealthBar HealthBar1 { get; }
    private UIImage Padding1 { get; }
    private UIText TextName2 { get; }
    private UIText TextName3 { get; }
    private ModHealthBar HealthBar2 { get; }
    private UIImage Padding2 { get; }
    private UIText TextName4 { get; }

    private Vector2 OriginalPivot { get; }
    private int OriginalFontSize { get; }
    private int BaseFontSize { get; }


    private static ModConfigHoverGuide Config => Mod.Config.HoverGuide;
    private static ModConfigHoverGuideColorSet ColorConfig => Config.ColorSet;
    private static ModConfigHoverGuideStyleChara StyleConfig => Config.CurrentStyle.Chara;

    public ModHoverGuide(WidgetMouseover widget)
    {
        var localScale = widget.textName.transform.localScale;

        HealthBar1 = new(widget);

        Padding1 = AddPadding(widget.layout, ModConsts.GameObjectName.HoverGuidePadding, localScale);

        TextName2 = UnityEngine.Object.Instantiate(widget.textName);
        TextName2.transform.SetParent(widget.layout.transform);
        TextName2.transform.localScale = localScale;

        TextName3 = UnityEngine.Object.Instantiate(widget.textName);
        TextName3.transform.SetParent(widget.layout.transform);
        TextName3.transform.localScale = localScale;

        HealthBar2 = new(widget);

        Padding2 = AddPadding(widget.layout, ModConsts.GameObjectName.HoverGuidePadding, localScale);

        TextName4 = UnityEngine.Object.Instantiate(widget.textName);
        TextName4.transform.SetParent(widget.layout.transform);
        TextName4.transform.localScale = localScale;

        OriginalPivot = widget.layout.Rect().pivot;
        // ゲーム設定のウィジェットのフォントサイズが "すごく小さい" (最小値) の場合を基準のフォントサイズとする
        OriginalFontSize = widget.textName.fontSize;
        BaseFontSize = widget.textName.fontSize - EClass.core.config.font.fontWidget.size;

        // ウィジェットを無効から有効に切り替えた際に表示が乱れないようにするため、
        // 初期状態では追加コンポーネントなどは表示しないようにする
        HealthBar1.Enabled = false;
        DisablePadding(Padding1);
        TextName2.enabled = false;
        TextName3.enabled = false;
        HealthBar2.Enabled = false;
        DisablePadding(Padding2);
        TextName4.enabled = false;
    }

    private static UIImage AddPadding(LayoutGroup layout, string name, Vector3 localScale)
    {
        // GameObjectを生成し、layoutに挿入する
        var obj = new GameObject(name);
        // 体力バーの画像を設定する
        var image = obj.AddComponent<UIImage>();
        image.transform.Rect().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        obj.transform.SetParent(layout.transform);
        obj.transform.localScale = localScale;

        return image;
    }

    private void DisablePadding(UIImage padding)
    {
        UpdatePadding(padding, false, 0);
    }

    private void UpdatePadding(UIImage padding, bool enabled, float height)
    {
        padding.enabled = enabled;
        padding.transform.Rect().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, enabled ? height : 0);
    }

    public void Show(WidgetMouseover widget, string? text, string? text2, string? text3, string? text4, Card? target1, Card? target2)
    {
        var fontColor = widget.textName.fontColor;
        var fontSize = ModUIUtil.ComputeFontSize(BaseFontSize);
        var paddingHeight = PaddingHeight * fontSize / BaseFontSize;

        var isGroup1Enabled = false;
        var isPaddingRequired = false;
        if (!string.IsNullOrEmpty(text))
        {
            text = text?.TagSize(fontSize).TagColorNullable(ColorConfig.MainTextColor);
            widget.textName.fontSize = fontSize;
            widget.textName.enabled = true;
            isGroup1Enabled = true;
            isPaddingRequired = true;
        }
        else
        {
            widget.textName.enabled = false;
        }
        if (target1 is Chara chara1 && (!StyleConfig.EnableMimicry || !chara1.HasMimicryThing))
        {
            var enabled = DisplaysHealthBar(chara1);
            HealthBar1.Enabled = enabled;
            HealthBar1.Update(chara1);
            isGroup1Enabled = enabled;
            isPaddingRequired = !enabled;
        }
        else
        {
            HealthBar1.Enabled = false;
        }
        if (!string.IsNullOrEmpty(text2))
        {
            text2 = text2?.TagColorNullable(ColorConfig.MainTextColor);
            TextName2.fontColor = fontColor;
            TextName2.fontSize = fontSize;
            TextName2.text = text2.TagSize(fontSize);
            TextName2.enabled = true;
            isGroup1Enabled = true;
        }
        else
        {
            TextName2.text = string.Empty;
            TextName2.enabled = false;
            isPaddingRequired = false;
        }

        UpdatePadding(Padding1, isPaddingRequired, paddingHeight);

        isPaddingRequired = false;
        if (!string.IsNullOrEmpty(text3))
        {
            if (isGroup1Enabled)
            {
                text3 = $"{Environment.NewLine}{text3}";
            }
            text3 = text3?.TagColorNullable(ColorConfig.MainTextColor);
            TextName3.fontColor = fontColor;
            TextName3.fontSize = fontSize;
            TextName3.text = text3.TagSize(fontSize);
            TextName3.enabled = true;
            isPaddingRequired = true;
        }
        else
        {
            TextName3.text = string.Empty;
            TextName3.enabled = false;
        }
        if (target2 is Chara chara2 && (!StyleConfig.EnableMimicry || !chara2.HasMimicryThing))
        {
            var enabled = DisplaysHealthBar(chara2);
            HealthBar2.Enabled = enabled;
            HealthBar2.Update(chara2);
            isPaddingRequired = !enabled;
        }
        else
        {
            HealthBar2.Enabled = false;
        }
        if (!string.IsNullOrEmpty(text4))
        {
            text4 = text4?.TagColorNullable(ColorConfig.MainTextColor);
            TextName4.fontColor = fontColor;
            TextName4.fontSize = fontSize;
            TextName4.text = text4.TagSize(fontSize);
            TextName4.enabled = true;
        }
        else
        {
            TextName4.text = string.Empty;
            TextName4.enabled = false;
            isPaddingRequired = false;
        }

        UpdatePadding(Padding2, isPaddingRequired, paddingHeight);

        widget.Show(text);

        widget.layout.Rect().pivot = new(Config.HorizontalPivot, Config.VerticalPivot);
    }

    public void ShowForManager(WidgetMouseover widget)
    {
        widget.layout.Rect().pivot = OriginalPivot;
        widget.textName.fontSize = OriginalFontSize;
        widget.textName.enabled = true;
        HealthBar1.Enabled = false;
        DisablePadding(Padding1);
        TextName2.enabled = false;
        TextName3.enabled = false;
        HealthBar2.Enabled = false;
        DisablePadding(Padding2);
        TextName4.enabled = false;
        widget.layout.RebuildLayout();
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

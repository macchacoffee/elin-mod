using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using SomewhatEnhancedDisplay.Extensions;
using SomewhatEnhancedDisplay.Config;
using ModUtility.Util;
using System.Net.Sockets;

namespace SomewhatEnhancedDisplay.UI.HoverGuide;

public class ModHealthBar
{
    private static readonly float Height = 24;
    private static readonly float BarHeight = 6;
    private static readonly int ValueFontSize = 13;
    private GameObject LayoutObj { get; }
    public LayoutElement Layout { get; }
    private UIImage BGImage { get; }
    private UIImage FGDamageImage { get; }
    private UIImage FGRestoreImage { get; }
    private UIImage FGImage { get; }
    private UIText ValueText { get; }
    private float ValueRatio { get; set; }
    private WeakReference<Chara?> Target { get; }
    private Tween? FGImageTween { get; set; }
    private Tween? FGDamageImageTween { get; set; }

    private static ModConfigHoverGuide Config => Mod.Config.HoverGuide;
    private static ModConfigHoverGuideColorSet ColorConfig => Config.ColorSet;
    private static ModConfigHoverGuideStyleChara StyleConfig => Config.CurrentStyle.Chara;

    public ModHealthBar(WidgetMouseover widget)
    {
        Target = new(null);
        var localScale = widget.textName.transform.localScale;
        var font = widget.textName.font;

        LayoutObj = new GameObject(ModConsts.GameObjectName.HealthBar);
        Layout = LayoutObj.AddComponent<LayoutElement>();
        LayoutObj.transform.SetParent(widget.layout.transform);
        LayoutObj.transform.localScale = localScale;

        BGImage = AddHealthBarImage(Layout, ModConsts.GameObjectName.HealthBarBG, localScale, ColorConfig.HealthBarBGColor);
        FGDamageImage = AddHealthBarImage(Layout, ModConsts.GameObjectName.HealthBarFGDamage, localScale, ColorConfig.HealthBarBGColor);
        FGRestoreImage = AddHealthBarImage(Layout, ModConsts.GameObjectName.HealthBarFGRestore, localScale, ColorConfig.HealthBarFGColor);
        FGImage = AddHealthBarImage(Layout, ModConsts.GameObjectName.HealthBarFG, localScale, ColorConfig.HealthBarFGColor);

        var valueObj = new GameObject(ModConsts.GameObjectName.HealthBarValue);
        ValueText = valueObj.AddComponent<UIText>();
        ValueText.enabled = StyleConfig.HealthBar.DisplayValue;
        ValueText.supportRichText = true;
        ValueText.font = font;
        ValueText.text = string.Empty;
        ValueText.alignment = TextAnchor.MiddleCenter;
        valueObj.transform.SetParent(Layout.transform);
        valueObj.transform.localScale = localScale;

        var valueShadow = valueObj.AddComponent<Shadow>();
        valueShadow.effectColor = new Color(0, 0, 0);
        valueShadow.effectDistance = new Vector2(1, -1);
        valueShadow.useGraphicAlpha = true;
    }

    private static UIImage AddHealthBarImage(LayoutElement layout, string name, Vector3 localScale, Color color)
    {
        // GameObjectを生成し、layoutに挿入する
        var obj = new GameObject(name);
        // 体力バーの画像を設定する
        var image = obj.AddComponent<UIImage>();
        image.sprite = ModUIUtil.Create1x1WhiteSprite();
        image.color = color;
        image.type = Image.Type.Filled;
        image.fillOrigin = (int)Image.OriginHorizontal.Left;
        image.fillMethod = Image.FillMethod.Horizontal;
        obj.transform.SetParent(layout.transform);
        obj.transform.localScale = localScale;

        return image;
    }

    private void UpdateTransformSize(Component component, float width, float height)
    {
        var rect = component.transform.Rect();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public void Update(Chara chara)
    {
        var ratio = chara.HealthRatio;
        // 0%または100%以上の場合は小数点以下なし、それ以外の場合は小数第1位まで表示する
        // 現在HPが最大HPよりも1でも低ければ100%とは表示しないようにする
        var pct = ModMath.Ceiling((ratio < 1 ? Math.Min(ratio, 0.999f) : ratio) * 100, 1);
        var pctColor = Color.Lerp(ColorConfig.HealthBarLowValueTextColor, ColorConfig.HealthBarTextColor, ratio);
        var barColor = Color.Lerp(ColorConfig.HealthBarLowValueFGColor, ColorConfig.HealthBarFGColor, ratio);
        var pctText = pct == 0 || pct >= 100 ? $"{pct:0}" : $"{pct:0.0}";

        ValueText.text = $"{pctText}%".TagColor(pctColor);

        // FGImage        増加: 低速, 低下: 一瞬
        // FGDamageImage  増加: 一瞬, 低下: 低速
        // FGRestoreImage 増加: 一瞬, 低下: 一瞬
        // 設定で有効な場合は体力バーの増減をアニメーションで表現する
        if (!StyleConfig.HealthBar.UseAnimation
            || !Layout.IsActive()
            || !Target.TryGetTarget(out var target) || target != chara)
        {
            // 対象が変わった場合も体力の割合が変わりうるため、
            // 体力バーのアニメーションを行わないようにする
            FGImageTween?.Kill(true);
            FGDamageImageTween?.Kill(true);
            FGDamageImage.fillAmount = ratio;
            FGRestoreImage.fillAmount = ratio;
            FGImage.fillAmount = ratio;
            FGDamageImage.color = ColorConfig.HealthBarBGColor;
            FGRestoreImage.color = ColorConfig.HealthBarBGColor;
            FGImage.color = barColor;
        } else if (ValueRatio != ratio)
        {
            UpdateFGImage(ratio, barColor);
            UpdateFGDamageImage(ratio, barColor);
            UpdateFGRestoreImage(ratio);
        }
        ValueRatio = ratio;

        Target.SetTarget(chara);
    }

    private void UpdateFGImage(float valueRatio, Color barColor)
    {
        var ratioDelta = valueRatio - FGImage.fillAmount;
        if (ratioDelta == 0)
        {
            return;
        }
        FGImageTween?.Kill();
        if (ratioDelta < 0)
        {
            return;
        }

        var duration = Math.Abs(ratioDelta) * 1.5f;
        FGImageTween = DOTween.Sequence()
            .SetLink(LayoutObj)
            .Join(
                FGImage
                .DOFillAmount(valueRatio, duration)
                .SetLink(LayoutObj)
                .SetDelay(0.1f)
                .SetEase(Ease.Linear))
            .Join(
                FGImage
                .DOColor(barColor, duration)
                .SetLink(LayoutObj)
                .SetDelay(0.1f)
                .SetEase(Ease.Linear))
        .OnStart(() =>
        {
            if (valueRatio > FGDamageImage.fillAmount)
            {
                FGDamageImage.fillAmount = valueRatio;
            }
            FGRestoreImage.color = ColorConfig.HealthBarFGRestoreColor;
        })
        .OnKill(() =>
        {
            FGRestoreImage.color = ColorConfig.HealthBarBGColor;
        });
    }

    private void UpdateFGDamageImage(float valueRatio, Color barColor)
    {
        var ratioDelta = valueRatio - FGDamageImage.fillAmount;
        if (ratioDelta == 0)
        {
            return;
        }
        FGDamageImageTween?.Kill();
        if (ratioDelta > 0)
        {
            return;
        }

        var duration = Math.Abs(ratioDelta) * 3;
        FGDamageImageTween = FGDamageImage
            .DOFillAmount(valueRatio, duration)
            .SetLink(LayoutObj)
            .SetDelay(0.1f)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                if (valueRatio < FGImage.fillAmount)
                {
                    FGImage.fillAmount = valueRatio;
                    FGImage.color = barColor;
                }
                FGDamageImage.color = ColorConfig.HealthBarFGDamageColor;
            })
            .OnKill(() =>
            {
                FGDamageImage.color = ColorConfig.HealthBarBGColor;
            });
    }

    private void UpdateFGRestoreImage(float valueRatio)
    {
        FGRestoreImage.fillAmount = valueRatio;
    }

    public bool Enabled
    {
        get
        {
            return Layout.enabled;
        }
        set
        {
            Layout.enabled = value;
            BGImage.enabled = value;
            FGImage.enabled = value;
            FGDamageImage.enabled = value;
            FGRestoreImage.enabled = value;
            ValueText.enabled = value && StyleConfig.HealthBar.DisplayValue;
            UpdateSize(value);
        }
    }

    private void UpdateSize(bool enabled)

    {
        var fontSize = ModUIUtil.ComputeFontSize(ValueFontSize);
        ValueText.fontSize = fontSize;
        var width = 0f;
        var height = 0f;
        var barHeight = 0f;
        if (enabled)
        {
            var sizeRatio = (float)fontSize / ValueFontSize;
            width = StyleConfig.HealthBar.Width * sizeRatio;
            height = Height * sizeRatio;
            barHeight = BarHeight * sizeRatio;
        }

        UpdateTransformSize(Layout, width, height);
        UpdateTransformSize(BGImage, width, barHeight);
        UpdateTransformSize(FGImage, width, barHeight);
        UpdateTransformSize(FGDamageImage, width, barHeight);
        UpdateTransformSize(FGRestoreImage, width, barHeight);
        UpdateTransformSize(ValueText, width, height);
    }
}

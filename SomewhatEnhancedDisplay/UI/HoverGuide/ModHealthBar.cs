using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using SomewhatEnhancedDisplay.Extensions;
using SomewhatEnhancedDisplay.Config;
using ModUtility.Util;

namespace SomewhatEnhancedDisplay.UI.HoverGuide;

public class ModHealthBar
{
    private static readonly float Height = 24;
    private static readonly float BarHeight = 6;
    private static readonly int ValueFontSize = 13;
    private static readonly float TweenDelay = 0.1f;
    private GameObject LayoutObj { get; }
    public LayoutElement Layout { get; }
    private UIImage BGImage { get; }
    private UIImage FGDamageImage { get; }
    private UIImage FGRestoreImage { get; }
    private UIImage FGImage { get; }
    private UIText ValueText { get; }
    private float ValueRatio { get; set; }
    private WeakReference<Chara?> Target { get; }
    private Tween? FGRestoreTween { get; set; }
    private Tween? FGDamageTween { get; set; }

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
        var pctColor = Color.Lerp(ColorConfig.HealthBarLowValueTextColor, ColorConfig.HealthBarTextColor, ratio);
        var barColor = Color.Lerp(ColorConfig.HealthBarLowValueFGColor, ColorConfig.HealthBarFGColor, ratio);

        // 設定で有効な場合は体力バーの増減をアニメーションで表現する
        if (!StyleConfig.HealthBar.UseAnimation
            || !Layout.IsActive()
            || !Target.TryGetTarget(out var target) || target != chara)
        {
            // 対象が変わった場合も体力の割合が変わりうるため、
            // 体力バーのアニメーションを行わないようにする
            FGRestoreTween?.Kill(true);
            FGDamageTween?.Kill(true);
            ValueText.text = GetValueText(ratio).TagColor(pctColor);
            ValueText.color = pctColor;
            FGDamageImage.fillAmount = ratio;
            FGRestoreImage.fillAmount = ratio;
            FGImage.fillAmount = ratio;
            FGDamageImage.color = ColorConfig.HealthBarBGColor;
            FGRestoreImage.color = ColorConfig.HealthBarBGColor;
            FGImage.color = barColor;
        } else if (ValueRatio != ratio)
        {
            UpdateRestore(ratio, barColor, pctColor);
            UpdateDamage(ratio, barColor, pctColor);
        }
        ValueRatio = ratio;

        Target.SetTarget(chara);
    }

    private static string GetValueText(float ratio)
    {
        // 0%または100%以上の場合は小数点以下なし、それ以外の場合は小数第1位まで表示する
        // 現在HPが最大HPよりも1でも低ければ100%とは表示しないようにする
        var pct = ModMath.Ceiling((ratio < 1 ? Math.Min(ratio, 0.999f) : ratio) * 100, 1);
        var pctText = pct == 0 || pct >= 100 ? $"{pct:0}" : $"{pct:0.0}";
        return $"{pctText}%";
    }

    private void UpdateRestore(float valueRatio, Color barColor, Color textColor)
    {
        float ratio1 = Math.Min(1, valueRatio);
        FGRestoreImage.fillAmount = ratio1;

        var ratioDelta = valueRatio - FGImage.fillAmount;
        if (ratioDelta == 0)
        {
            return;
        }
        var hasOldTween = FGRestoreTween?.IsPlaying() ?? false;
        FGRestoreTween?.Kill();
        if (ratioDelta < 0)
        {
            return;
        }

        var barRatio = FGImage.fillAmount;
        var duration = Math.Abs(ratio1 - barRatio) * 1.5f;
        FGRestoreTween = DOTween.Sequence()
            .SetLink(LayoutObj)
            .Join(
                FGImage
                .DOFillAmount(ratio1, duration)
                .SetLink(LayoutObj)
                .SetDelay(hasOldTween ? 0 : TweenDelay)
                .SetEase(Ease.Linear))
            .Join(
                FGImage
                .DOColor(barColor, duration)
                .SetLink(LayoutObj)
                .SetDelay(hasOldTween ? 0 : TweenDelay)
                .SetEase(Ease.Linear))
            .Join(
                DOTween.To(
                    () => barRatio,
                    value => ValueText.text = GetValueText(value),
                    valueRatio,
                    duration)
                .SetLink(LayoutObj)
                .SetDelay(hasOldTween ? 0 : TweenDelay)
                .SetEase(Ease.Linear))
            .Join(
                ValueText
                .DOColor(textColor, duration)
                .SetLink(LayoutObj)
                .SetDelay(hasOldTween ? 0 : TweenDelay)
                .SetEase(Ease.Linear))
        .OnStart(() =>
        {
            if (valueRatio > FGDamageImage.fillAmount)
            {
                FGDamageImage.fillAmount = ratio1;
            }
            FGRestoreImage.color = ColorConfig.HealthBarFGRestoreColor;
        })
        .OnComplete(() =>
        {
            FGRestoreImage.color = ColorConfig.HealthBarBGColor;
        });
    }

    private void UpdateDamage(float valueRatio, Color barColor, Color textColor)
    {
        float ratio1 = Math.Min(1, valueRatio);
        var ratioDelta = valueRatio - FGDamageImage.fillAmount;
        if (ratioDelta == 0)
        {
            return;
        }
        var hasOldTween = FGDamageTween?.IsPlaying() ?? false;
        FGDamageTween?.Kill();
        if (ratioDelta > 0)
        {
            return;
        }

        var barRatio = FGDamageImage.fillAmount;
        var duration = Math.Abs(ratio1 - barRatio) * 3;
        FGDamageTween = FGDamageImage
            .DOFillAmount(ratio1, duration)
            .SetLink(LayoutObj)
            .SetDelay(hasOldTween ? 0 : TweenDelay)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                if (valueRatio < FGImage.fillAmount)
                {
                    FGImage.fillAmount = ratio1;
                    FGImage.color = barColor;
                    ValueText.text = GetValueText(valueRatio);
                    ValueText.color = textColor;
                }
                FGDamageImage.color = ColorConfig.HealthBarFGDamageColor;
            })
            .OnComplete(() =>
            {
                FGDamageImage.color = ColorConfig.HealthBarBGColor;
            });
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

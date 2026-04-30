using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using ModUtility.Util;
using SomewhatEnhancedDisplay.Extensions;
using SomewhatEnhancedDisplay.Config;

namespace SomewhatEnhancedDisplay.UI.HoverGuide;

public class ModHealthBar
{
    private static readonly float Height = 32;
    private static readonly float BarHeight = 8;
    private static readonly int ValueFontSize = 16;
    private static readonly float TweenDelay = 0.1f;
    private GameObject LayoutObj { get; }
    public LayoutElement Layout { get; }
    private UIImage BGImage { get; }
    private UIImage FGDamageImage { get; }
    private UIImage FGRestoreImage { get; }
    private UIImage FGImage { get; }
    private UIText ValueText { get; }
    private double ValueRatio { get; set; }
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

        LayoutObj = new GameObject(ModConsts.GameObjectName.HealthBar);
        Layout = LayoutObj.AddComponent<LayoutElement>();
        LayoutObj.transform.SetParent(widget.layout.transform);
        LayoutObj.transform.localScale = localScale;

        BGImage = AddHealthBarImage(Layout.transform, ModConsts.GameObjectName.HealthBarBG, localScale, ColorConfig.HealthBarBGColor);
        FGDamageImage = AddHealthBarImage(Layout.transform, ModConsts.GameObjectName.HealthBarFGDamage, localScale, ColorConfig.HealthBarBGColor);
        FGRestoreImage = AddHealthBarImage(Layout.transform, ModConsts.GameObjectName.HealthBarFGRestore, localScale, ColorConfig.HealthBarFGColor);
        FGImage = AddHealthBarImage(Layout.transform, ModConsts.GameObjectName.HealthBarFG, localScale, ColorConfig.HealthBarFGColor);

        var valueObj = new GameObject(ModConsts.GameObjectName.HealthBarValue);
        ValueText = valueObj.AddComponent<UIText>();
        ValueText.enabled = StyleConfig.HealthBar.DisplayValue;
        ValueText.supportRichText = true;
        ValueText.font = widget.textName.font;
        ValueText.fontType = FontType.Widget;
        ValueText.text = string.Empty;
        ValueText.alignment = TextAnchor.MiddleCenter;
        valueObj.transform.SetParent(Layout.transform);
        valueObj.transform.localScale = localScale;

        var valueShadow = valueObj.AddComponent<Shadow>();
        valueShadow.effectColor = new Color(0, 0, 0);
        valueShadow.effectDistance = new Vector2(1, -1);
        valueShadow.useGraphicAlpha = true;
    }

    private static UIImage AddHealthBarImage(Transform transform, string name, Vector3 localScale, Color color)
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
        obj.transform.SetParent(transform);
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
        Update(chara, null);
    }

    public void Update(Chara chara, ModHoverGuideTargetModifier? modifier)
    {
        var ratio = modifier?.HealthBarRatio ?? chara.HealthRatio;
        var pctColor = Color.Lerp(ColorConfig.HealthBarLowValueTextColor, ColorConfig.HealthBarTextColor, (float)ratio);
        var barColor = Color.Lerp(ColorConfig.HealthBarLowValueFGColor, ColorConfig.HealthBarFGColor, (float)ratio);

        // 設定で有効な場合は体力バーの増減をアニメーションで表現する
        if (!StyleConfig.HealthBar.UseAnimation
            || !Layout.IsActive()
            || !Target.TryGetTarget(out var target) || target != chara)
        {
            // 対象が変わった場合も体力の割合が変わりうるため、
            // 体力バーのアニメーションを行わないようにする
            FGRestoreTween?.Kill(true);
            FGDamageTween?.Kill(true);
            ValueText.text = GetValueText(ratio);
            ValueText.color = pctColor;
            FGDamageImage.fillAmount = (float)ratio;
            FGRestoreImage.fillAmount = (float)ratio;
            FGImage.fillAmount = (float)ratio;
            FGDamageImage.color = ColorConfig.HealthBarBGColor;
            FGRestoreImage.color = ColorConfig.HealthBarBGColor;
            FGImage.color = barColor;
        }
        else if (ValueRatio != ratio)
        {
            UpdateRestore(ratio, barColor, pctColor);
            UpdateDamage(ratio, barColor, pctColor);
        }
        else if (!(FGRestoreTween?.IsPlaying() ?? false))
        {
            // 色の設定変更が即座に反映されるようにするため、
            // 回復アニメーションが再生されていない場合は文字とバー画像の色を変更する
            ValueText.color = pctColor;
            FGImage.color = barColor;
        }
        ValueRatio = ratio;

        Target.SetTarget(chara);
    }

    private static double RoundRatioForValueText(double ratio)
    {
        // %表記で小数第1位まで表示するため、小数第3位以降を丸める
        // 現在HPが最大HPよりも1でも低ければ0.999になるようにする
        return ModMath.Ceiling(ratio < 1 ? Math.Min(ratio, 0.999) : ratio, 3);
    }

    private static string GetValueText(double ratio)
    {
        var pct = RoundRatioForValueText(ratio) * 100;
        // 0%または100%以上の場合は小数点以下なし、それ以外の場合は小数第1位まで表示する
        var pctText = pct == 0 || pct >= 100 ? $"{pct:0}" : $"{pct:0.0}";
        return $"{pctText}%";
    }

    private void UpdateRestore(double valueRatio, Color barColor, Color textColor)
    {
        var ratio = Math.Min(1, valueRatio);
        FGRestoreImage.fillAmount = (float)ratio;
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
        var duration = Math.Abs((float)ratio - barRatio) * 1.5f;
        FGRestoreTween = DOTween.Sequence()
            .SetLink(LayoutObj)
            .Join(
                FGImage
                .DOFillAmount((float)ratio, duration)
                .SetLink(LayoutObj)
                .SetEase(Ease.Linear))
            .Join(
                FGImage
                .DOColor(barColor, duration)
                .SetLink(LayoutObj)
                .SetEase(Ease.Linear))
            .Join(
                DOTween.To(
                    () => (double)barRatio,
                    value => ValueText.text = GetValueText(value),
                    RoundRatioForValueText(valueRatio),
                    duration)
                .SetLink(LayoutObj)
                .SetEase(Ease.Linear))
            .Join(
                ValueText
                .DOColor(textColor, duration)
                .SetLink(LayoutObj)
                .SetEase(Ease.Linear))
        .SetLink(LayoutObj)
        .SetDelay(hasOldTween ? 0 : TweenDelay)
        .OnStart(() =>
        {
            if (valueRatio > FGDamageImage.fillAmount)
            {
                FGDamageImage.fillAmount = (float)ratio;
            }
            FGRestoreImage.color = ColorConfig.HealthBarFGRestoreColor;
        })
        .OnComplete(() =>
        {
            FGRestoreImage.color = ColorConfig.HealthBarBGColor;
        });
    }

    private void UpdateDamage(double valueRatio, Color barColor, Color textColor)
    {
        var ratio = Math.Min(1, valueRatio);
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
        var duration = Math.Abs((float)ratio - barRatio) * 3;
        FGDamageTween = FGDamageImage
            .DOFillAmount((float)ratio, duration)
            .SetLink(LayoutObj)
            .SetDelay(hasOldTween ? 0 : TweenDelay)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                if (valueRatio < FGImage.fillAmount)
                {
                    FGImage.fillAmount = (float)ratio;
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

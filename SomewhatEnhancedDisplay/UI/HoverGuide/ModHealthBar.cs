using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using ModUtility.Util;
using SomewhatEnhancedDisplay.Extensions;
using SomewhatEnhancedDisplay.Config;

namespace SomewhatEnhancedDisplay.UI.HoverGuide;

internal class ModHealthBar
{
    private const float _height = 32;
    private const float _barHeight = 8;
    private const int _valueFontSize = 16;
    private const float _tweenDelay = 0.1f;

    private GameObject LayoutObj { get; }
    public LayoutElement Layout { get; }
    private UIImage BGImage { get; }
    private UIImage FGDamageImage { get; }
    private UIImage FGRestoreImage { get; }
    private UIImage FGImage { get; }
    private UIText ValueText { get; }

    private double ValueRatio { get; set; }
    private WeakReference<Chara?> Target { get; }
    private ModHoverGuideTargetModifier? TargetModifier { get; set; }

    private Tween? FGRestoreTween { get; set; }
    private Tween? FGDamageTween { get; set; }

    private bool? _lastSizeEnabled;
    private int _lastFontSize = -1;
    private float _lastWidth = float.NaN;

    public bool Enabled => Layout.enabled;

    private static ModConfigHoverGuide Config => ModContext.WorldConfig.HoverGuide;
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
        FGRestoreImage = AddHealthBarImage(Layout.transform, ModConsts.GameObjectName.HealthBarFGRestore, localScale, ColorConfig.HealthBarBGColor);
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
        // GameObjectを生成し、layoutに挿入する。
        var obj = new GameObject(name);
        // 体力バーの画像を設定する。
        var image = obj.AddComponent<UIImage>();
        image.sprite = ModUIUtil.White1x1Sprite;
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

    public void Update()
    {
        if (!Target.TryGetTarget(out var target) || target is null)
        {
            return;
        }

        UpdateValue(target, TargetModifier, false);
    }

    public void UpdateTarget(Chara chara)
    {
        UpdateTarget(chara, null);
    }

    public void UpdateTarget(Chara chara, ModHoverGuideTargetModifier? modifier)
    {
        var targetChanged = !Target.TryGetTarget(out var target) || target != chara;

        Target.SetTarget(chara);
        TargetModifier = modifier;

        UpdateValue(chara, modifier, targetChanged);
        UpdatePresentation(chara, modifier);
    }

    private void UpdateValue(Chara chara, ModHoverGuideTargetModifier? modifier, bool targetChanged)
    {
        if (TryHandleDeadTarget(chara))
        {
            return;
        }

        var ratio = GetHealthRatio(chara, modifier);

        if (!StyleConfig.HealthBar.UseAnimation || targetChanged)
        {
            SetValueImmediately(ratio);
        }
        else if (ValueRatio != ratio)
        {
            var textColor = GetTextColor(ratio);
            var barColor = GetBarColor(ratio);
            UpdateRestore(ratio, barColor, textColor);
            UpdateDamage(ratio,barColor, textColor);
        }

        ValueRatio = ratio;
    }

    private void UpdatePresentation(Chara chara, ModHoverGuideTargetModifier? modifier)
    {
        var enabled = Displays(chara, modifier);
        if (Layout.enabled != enabled)
        {
            Layout.enabled = enabled;
        }

        UpdateSize(enabled);

        ValueText.enabled = StyleConfig.HealthBar.DisplayValue;

        if (!(FGRestoreTween?.IsPlaying() ?? false))
        {
            // 色の設定変更が即座に反映されるようにするため、
            // 回復アニメーションが再生されていない場合は文字とバー画像の色を変更する。
            var ratio = GetHealthRatio(chara, modifier);
            ValueText.color =  GetTextColor(ratio);
            FGImage.color =  GetBarColor(ratio);
        }
    }

    private bool TryHandleDeadTarget(Chara chara)
    {
        if (!chara.isDead)
        {
            return false;
        }
        if (ValueRatio != 0 || FGRestoreTween?.IsPlaying() == true || FGDamageTween?.IsPlaying() == true)
        {
            SetValueImmediately(0);
        }

        ValueRatio = 0;

        return true;
    }

    public void Hide()
    {
        if (Layout.enabled)
        {
            Layout.enabled = false;
        }
        UpdateSize(false);
    }

    public void StopTracking()
    {
        if (!Target.TryGetTarget(out var target) || target is null || !TryHandleDeadTarget(target))
        {
            FGRestoreTween?.Kill();
            FGDamageTween?.Kill();
            FGRestoreTween = null;
            FGDamageTween = null;
        }

        Target.SetTarget(null);
        TargetModifier = null;
    }

    private static double GetHealthRatio(Chara chara, ModHoverGuideTargetModifier? modifier)
    {
        return modifier?.HealthBarRatio ?? chara.HealthRatio;
    }

    private static Color GetTextColor(double ratio)
    {
        return Color.Lerp(ColorConfig.HealthBarLowValueTextColor, ColorConfig.HealthBarTextColor, (float)ratio);
    }

    private static Color GetBarColor(double ratio)
    {
        return Color.Lerp(ColorConfig.HealthBarLowValueFGColor, ColorConfig.HealthBarFGColor, (float)ratio);
    }

    private void SetValueImmediately(double ratio)
    {
        SetValueImmediately(ratio, GetBarColor(ratio), GetTextColor(ratio));
    }

    private void SetValueImmediately(double ratio, Color barColor, Color textColor)
    {
        FGRestoreTween?.Kill(true);
        FGDamageTween?.Kill(true);
        FGRestoreTween = null;
        FGDamageTween = null;

        ValueText.text = GetValueText(ratio);
        ValueText.color = textColor;

        FGDamageImage.fillAmount = (float)ratio;
        FGRestoreImage.fillAmount = (float)ratio;
        FGImage.fillAmount = (float)ratio;

        FGDamageImage.color = ColorConfig.HealthBarBGColor;
        FGRestoreImage.color = ColorConfig.HealthBarBGColor;
        FGImage.color = barColor;
    }

    private static double RoundRatioForValueText(double ratio)
    {
        // %表記で小数第1位まで表示するため、小数第3位以降を丸める。
        // 現在HPが最大HPよりも1でも低ければ0.999になるようにする。
        return ModMath.Ceiling(ratio < 1 ? Math.Min(ratio, 0.999) : ratio, 3);
    }

    private static string GetValueText(double ratio)
    {
        var pct = RoundRatioForValueText(ratio) * 100;
        // 0%または100%以上の場合は小数点以下なし、それ以外の場合は小数第1位まで表示する。
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
        FGRestoreTween = null;
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
        .SetDelay(hasOldTween ? 0 : _tweenDelay)
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
        FGDamageTween = null;
        if (ratioDelta > 0)
        {
            return;
        }

        if (valueRatio < FGImage.fillAmount)
        {
            FGImage.fillAmount = (float)ratio;
            FGImage.color = barColor;
            ValueText.text = GetValueText(valueRatio);
            ValueText.color = textColor;
        }
        FGDamageImage.color = ColorConfig.HealthBarFGDamageColor;

        var barRatio = FGDamageImage.fillAmount;
        var duration = Math.Abs((float)ratio - barRatio) * 3;
        FGDamageTween = FGDamageImage
            .DOFillAmount((float)ratio, duration)
            .SetLink(LayoutObj)
            .SetDelay(hasOldTween ? 0 : _tweenDelay)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                FGDamageImage.color = ColorConfig.HealthBarBGColor;
            });
    }

    private void UpdateSize(bool enabled)
    {
        var fontSize = ModUIUtil.ComputeFontSize(_valueFontSize);
        var configWidth = StyleConfig.HealthBar.Width;
        if (_lastSizeEnabled == enabled && _lastFontSize == fontSize && (!enabled || _lastWidth == configWidth))
        {
            return;
        }
        _lastSizeEnabled = enabled;
        _lastFontSize = fontSize;
        _lastWidth = configWidth;

        ValueText.fontSize = fontSize;

        var width = 0f;
        var height = 0f;
        var barHeight = 0f;
        if (enabled)
        {
            var sizeRatio = (float)fontSize / _valueFontSize;
            width = StyleConfig.HealthBar.Width * sizeRatio;
            height = _height * sizeRatio;
            barHeight = _barHeight * sizeRatio;
        }

        UpdateTransformSize(Layout, width, height);
        UpdateTransformSize(BGImage, width, barHeight);
        UpdateTransformSize(FGImage, width, barHeight);
        UpdateTransformSize(FGDamageImage, width, barHeight);
        UpdateTransformSize(FGRestoreImage, width, barHeight);
        UpdateTransformSize(ValueText, width, height);
    }

    private bool Displays(Chara chara, ModHoverGuideTargetModifier? modifier)
    {
        if (!StyleConfig.DisplayHealthBar)
        {
            return false;
        }

        var config = StyleConfig.HealthBar.GetDisplayForChara(chara);
        if (config is null)
        {
            return false;
        }

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

        var isInFullHealth = modifier is not null ? modifier.HealthBarRatio >= 1 : chara.IsInFullHealth;
        return config.InFullHealth || FGImage.fillAmount < 1 || !isInFullHealth;
    }
}

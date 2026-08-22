using System;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

using Macchacoffee.ElinMods.ModUtility.Util;
using Macchacoffee.ElinMods.SomewhatEnhancedDisplay.Config;
using Macchacoffee.ElinMods.SomewhatEnhancedDisplay.Extensions;

namespace Macchacoffee.ElinMods.SomewhatEnhancedDisplay.UI.HoverGuide;

internal class HealthBar
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
    private UIImage FGSplitImage { get; }
    private UIText ValueText { get; }

    private double ValueRatio { get; set; }
    private double SplitPartRatio { get; set; }
    private bool SplitsManaBody { get; set; }
    private bool ReversesManaBody { get; set; }
    private WeakReference<Chara?> Target { get; }
    private HoverGuideTargetModifier? TargetModifier { get; set; }

    private Tween? FGRestoreTween { get; set; }
    private Tween? FGDamageTween { get; set; }

    private bool? _lastSizeEnabled;
    private int _lastFontSize = -1;
    private float _lastWidth = float.NaN;

    public bool Enabled => Layout.enabled;

    private static ModConfigHoverGuide Config => ModContext.WorldConfig.HoverGuide;
    private static ModConfigHoverGuideColorSet ColorConfig => Config.ColorSet;
    private static ModConfigHoverGuideStyleChara StyleConfig => Config.CurrentStyle.Chara;

    public HealthBar(WidgetMouseover widget)
    {
        Target = new(null);
        var localScale = widget.textName.transform.localScale;

        LayoutObj = new GameObject(ModConsts.GameObjectName.HealthBar);
        Layout = LayoutObj.AddComponent<LayoutElement>();
        LayoutObj.transform.SetParent(widget.layout.transform);
        LayoutObj.transform.localScale = localScale;

        BGImage = AddHealthBarImage(
            Layout.transform,
            ModConsts.GameObjectName.HealthBarBG,
            localScale,
            ColorConfig.HealthBarBGColor);
        FGDamageImage = AddHealthBarImage(
            Layout.transform,
            ModConsts.GameObjectName.HealthBarFGDamage,
            localScale,
            ColorConfig.HealthBarBGColor);
        FGRestoreImage = AddHealthBarImage(
            Layout.transform,
            ModConsts.GameObjectName.HealthBarFGRestore,
            localScale,
            ColorConfig.HealthBarBGColor);
        FGImage = AddHealthBarImage(
            Layout.transform,
            ModConsts.GameObjectName.HealthBarFG,
            localScale,
            ColorConfig.HealthBarFGColor);
        FGSplitImage = AddHealthBarImage(
            Layout.transform,
            ModConsts.GameObjectName.HealthBarFGSplit,
            localScale,
            ColorConfig.HealthBarFGColor);
        FGSplitImage.enabled = false;

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

    public void UpdateTarget(Chara chara, HoverGuideTargetModifier? modifier)
    {
        var targetChanged = !Target.TryGetTarget(out var target) || target != chara;

        Target.SetTarget(chara);
        TargetModifier = modifier;

        UpdateValue(chara, modifier, targetChanged);
        UpdatePresentation(chara, modifier);
    }

    private void UpdateValue(Chara chara, HoverGuideTargetModifier? modifier, bool targetChanged)
    {
        if (TryHandleDeadTarget(chara))
        {
            return;
        }

        var hasManaBodyPreview = modifier?.HasManaBodyHealthBarPreview == true;
        var splitsManaBody = ShouldSplitManaBody(chara, modifier, hasManaBodyPreview);
        var usesManaBodyPreview = splitsManaBody && hasManaBodyPreview;
        var ratio = GetHealthRatio(chara, modifier, usesManaBodyPreview);
        var reversesManaBody = splitsManaBody && StyleConfig.HealthBar.ReverseManaBodyHealthBar;
        var splitPartRatio = splitsManaBody
            ? GetSplitPartRatio(chara, modifier, usesManaBodyPreview, reversesManaBody)
            : 0;

        if (!StyleConfig.HealthBar.UseAnimation
            || targetChanged
            || SplitsManaBody != splitsManaBody
            || ReversesManaBody != reversesManaBody)
        {
            SetValueImmediately(ratio, splitPartRatio, splitsManaBody, reversesManaBody);
        }
        else if (ValueRatio != ratio)
        {
            var textColor = GetTextColor(ratio);
            var baseBarColor = GetBaseBarColor(ratio, splitsManaBody, reversesManaBody);
            var splitBarColor = GetSplitBarColor(ratio, reversesManaBody);
            UpdateRestore(ratio, splitPartRatio, splitsManaBody, baseBarColor, splitBarColor, textColor);
            UpdateDamage(ratio, splitPartRatio, splitsManaBody, baseBarColor, splitBarColor, textColor);
        }
        else if (SplitPartRatio != splitPartRatio)
        {
            SetSplitPartImmediately(splitPartRatio, GetSplitBarColor(ratio, reversesManaBody));
        }

        ValueRatio = ratio;
        SplitPartRatio = splitPartRatio;
        SplitsManaBody = splitsManaBody;
        ReversesManaBody = reversesManaBody;
        UpdateColors(ratio);
    }

    private void UpdatePresentation(Chara chara, HoverGuideTargetModifier? modifier)
    {
        var enabled = Displays(chara, modifier);
        if (Layout.enabled != enabled)
        {
            Layout.enabled = enabled;
        }

        UpdateSize(enabled);

        ValueText.enabled = StyleConfig.HealthBar.DisplayValue;
    }

    private void UpdateColors(double ratio)
    {
        if (!(FGRestoreTween?.IsPlaying() ?? false))
        {
            // 色の設定変更が即座に反映されるようにするため、
            // 回復アニメーションが再生されていない場合は文字とバー画像の色を変更する。
            ValueText.color = GetTextColor(ratio);
            FGImage.color = GetBaseBarColor(ratio, SplitsManaBody, ReversesManaBody);
            if (SplitsManaBody)
            {
                FGSplitImage.color = GetSplitBarColor(ratio, ReversesManaBody);
            }
        }
    }

    private bool TryHandleDeadTarget(Chara chara)
    {
        if (!chara.isDead)
        {
            return false;
        }
        if (ValueRatio != 0
            || SplitPartRatio != 0
            || SplitsManaBody
            || ReversesManaBody
            || FGImage.fillAmount != 0
            || FGRestoreTween?.IsPlaying() == true
            || FGDamageTween?.IsPlaying() == true)
        {
            SetValueImmediately(0, 0, false, false);
        }

        ValueRatio = 0;
        SplitPartRatio = 0;
        SplitsManaBody = false;
        ReversesManaBody = false;

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

    private static double GetHealthRatio(Chara chara, HoverGuideTargetModifier? modifier)
    {
        var hasManaBodyPreview = modifier?.HasManaBodyHealthBarPreview == true;
        var usesManaBodyPreview = hasManaBodyPreview
            && ShouldSplitManaBody(chara, modifier, hasManaBodyPreview);
        return GetHealthRatio(chara, modifier, usesManaBodyPreview);
    }

    private static double GetHealthRatio(
        Chara chara,
        HoverGuideTargetModifier? modifier,
        bool usesManaBodyPreview)
    {
        if (!usesManaBodyPreview)
        {
            return modifier?.HealthBarRatio ?? chara.HealthRatio;
        }

        GetEffectiveHealthValues(
            chara,
            modifier,
            usesManaBodyPreview,
            out var maxHP,
            out var maxMana,
            out var currentHP,
            out var currentMana
        );
        var maxTotal = maxHP + maxMana;
        return maxTotal > 0 ? (currentHP + currentMana) / maxTotal : 0;
    }

    private static bool ShouldSplitManaBody(
        Chara chara,
        HoverGuideTargetModifier? modifier,
        bool hasManaBodyPreview)
    {
        return StyleConfig.HealthBar.SplitManaBodyHealthBar
            && (modifier?.HealthBarRatio is null || hasManaBodyPreview)
            && chara.Evalue(FEAT.featManaMeat) > 0;
    }

    private static double GetSplitPartRatio(
        Chara chara,
        HoverGuideTargetModifier? modifier,
        bool usesManaBodyPreview,
        bool reversesManaBody)
    {
        GetEffectiveHealthValues(
            chara,
            modifier,
            usesManaBodyPreview,
            out var maxHP,
            out var maxMana,
            out var currentHP,
            out var currentMana
        );
        var splitBasis = Math.Max(maxHP + maxMana, currentHP + currentMana);
        if (splitBasis <= 0)
        {
            return 0;
        }

        var splitPartValue = reversesManaBody ? currentMana : currentHP;
        return splitPartValue / splitBasis;
    }

    private static void GetEffectiveHealthValues(
        Chara chara,
        HoverGuideTargetModifier? modifier,
        bool usesManaBodyPreview,
        out double maxHP,
        out double maxMana,
        out double currentHP,
        out double currentMana)
    {
        maxHP = Math.Max((long)chara.MaxHP, 0);
        maxMana = Math.Max((long)chara.mana.max, 0);
        if (usesManaBodyPreview)
        {
            currentHP = maxHP * modifier!.HealthBarHPRatio!.Value;
            currentMana = maxMana * modifier.HealthBarMPRatio!.Value;
            return;
        }

        currentHP = Math.Max((long)chara.hp, 0);
        currentMana = Math.Max((long)chara.mana.value, 0);
    }

    private static Color GetTextColor(double ratio)
    {
        return Color.Lerp(ColorConfig.HealthBarLowValueTextColor, ColorConfig.HealthBarTextColor, (float)ratio);
    }

    private static Color GetBarColor(double ratio)
    {
        return Color.Lerp(ColorConfig.HealthBarLowValueFGColor, ColorConfig.HealthBarFGColor, (float)ratio);
    }

    private static Color GetManaBarColor(double ratio)
    {
        return Color.Lerp(ColorConfig.HealthBarLowValueManaFGColor, ColorConfig.HealthBarManaFGColor, (float)ratio);
    }

    private static Color GetBaseBarColor(double ratio, bool splitsManaBody, bool reversesManaBody)
    {
        return splitsManaBody && !reversesManaBody ? GetManaBarColor(ratio) : GetBarColor(ratio);
    }

    private static Color GetSplitBarColor(double ratio, bool reversesManaBody)
    {
        return reversesManaBody ? GetManaBarColor(ratio) : GetBarColor(ratio);
    }

    private void SetValueImmediately(
        double ratio,
        double splitPartRatio,
        bool splitsManaBody,
        bool reversesManaBody)
    {
        SetValueImmediately(
            ratio,
            splitPartRatio,
            splitsManaBody,
            GetBaseBarColor(ratio, splitsManaBody, reversesManaBody),
            GetSplitBarColor(ratio, reversesManaBody),
            GetTextColor(ratio)
        );
    }

    private void SetValueImmediately(
        double ratio,
        double splitPartRatio,
        bool splitsManaBody,
        Color baseBarColor,
        Color splitBarColor,
        Color textColor)
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
        FGSplitImage.fillAmount = splitsManaBody ? (float)splitPartRatio : 0;
        FGSplitImage.enabled = splitsManaBody;

        FGDamageImage.color = ColorConfig.HealthBarBGColor;
        FGRestoreImage.color = ColorConfig.HealthBarBGColor;
        FGImage.color = baseBarColor;
        FGSplitImage.color = splitBarColor;
    }

    private void SetSplitPartImmediately(double splitPartRatio, Color splitBarColor)
    {
        if (FGRestoreTween?.IsPlaying() == true)
        {
            FGRestoreTween.Kill(true);
            FGRestoreTween = null;
        }
        FGSplitImage.fillAmount = (float)splitPartRatio;
        FGSplitImage.color = splitBarColor;
    }

    private static double RoundRatioForValueText(double ratio)
    {
        // %表記で小数第1位まで表示するため、小数第3位以降を丸める。
        // 現在HPが最大HPよりも1でも低ければ0.999になるようにする。
        return MathExtra.Ceiling(ratio < 1 ? Math.Min(ratio, 0.999) : ratio, 3);
    }

    private static string GetValueText(double ratio)
    {
        var pct = RoundRatioForValueText(ratio) * 100;
        // 0%または100%以上の場合は小数点以下なし、それ以外の場合は小数第1位まで表示する。
        var pctText = pct == 0 || pct >= 100 ? $"{pct:0}" : $"{pct:0.0}";
        return $"{pctText}%";
    }

    private void UpdateRestore(
        double valueRatio,
        double splitPartRatio,
        bool splitsManaBody,
        Color baseBarColor,
        Color splitBarColor,
        Color textColor)
    {
        var ratio = Math.Min(1, valueRatio);
        FGRestoreImage.fillAmount = (float)ratio;
        var ratioDelta = valueRatio - FGImage.fillAmount;
        if (ratioDelta == 0)
        {
            if (FGRestoreTween?.IsPlaying() == true)
            {
                FGRestoreTween.Kill();
                FGRestoreTween = null;
                FGRestoreImage.color = ColorConfig.HealthBarBGColor;
            }
            FGImage.color = baseBarColor;
            ValueText.text = GetValueText(valueRatio);
            ValueText.color = textColor;
            if (splitsManaBody)
            {
                FGSplitImage.fillAmount = (float)splitPartRatio;
                FGSplitImage.color = splitBarColor;
            }
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
        var sequence = DOTween.Sequence()
            .SetLink(LayoutObj)
            .Join(
                FGImage
                .DOFillAmount((float)ratio, duration)
                .SetLink(LayoutObj)
                .SetEase(Ease.Linear))
            .Join(
                FGImage
                .DOColor(baseBarColor, duration)
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
                .SetEase(Ease.Linear));
        if (splitsManaBody)
        {
            if (FGSplitImage.fillAmount != (float)splitPartRatio)
            {
                sequence.Join(
                    FGSplitImage
                    .DOFillAmount((float)splitPartRatio, duration)
                    .SetLink(LayoutObj)
                    .SetEase(Ease.Linear));
            }
            sequence.Join(
                FGSplitImage
                .DOColor(splitBarColor, duration)
                .SetLink(LayoutObj)
                .SetEase(Ease.Linear));
        }
        FGRestoreTween = sequence
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

    private void UpdateDamage(
        double valueRatio,
        double splitPartRatio,
        bool splitsManaBody,
        Color baseBarColor,
        Color splitBarColor,
        Color textColor)
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
            FGImage.color = baseBarColor;
            if (splitsManaBody)
            {
                FGSplitImage.fillAmount = (float)splitPartRatio;
                FGSplitImage.color = splitBarColor;
            }
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
        UpdateTransformSize(FGSplitImage, width, barHeight);
        UpdateTransformSize(FGDamageImage, width, barHeight);
        UpdateTransformSize(FGRestoreImage, width, barHeight);
        UpdateTransformSize(ValueText, width, height);
    }

    private bool Displays(Chara chara, HoverGuideTargetModifier? modifier)
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

        var isInFullHealth = modifier is not null ? GetHealthRatio(chara, modifier) >= 1 : chara.IsInFullHealth;
        return config.InFullHealth || FGImage.fillAmount < 1 || !isInFullHealth;
    }
}

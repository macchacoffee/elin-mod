using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using SomewhatEnhancedDisplay.Extensions;
using SomewhatEnhancedDisplay.Config;
using ModUtility.Util;

namespace SomewhatEnhancedDisplay.UI;

public class ModHealthBar
{
    private static readonly float Height = 24;
    private static readonly float BarHeight = 6;
    private static readonly int ValueFontSize = 13;

    private static readonly Texture2D BarTexture = CreateTexture(Color.white);

    private GameObject LayoutObj { get; }
    public LayoutElement Layout { get; }
    private UIImage BGImage { get; }
    private UIImage FGImage { get; }
    private UIImage FGDamageImage { get; }
    private UIText ValueText { get; }
    private Chara? Target { get; set; }
    private Tween? DamageTween { get; set; }

    private static ModConfigHoverGuide Config => Mod.Config.HoverGuide;
    private static ModConfigHoverGuideProfileChara ProfileConfig => Config.CurrentProfile.Chara;

    public ModHealthBar(WidgetMouseover widget)
    {
        var localScale = widget.textName.transform.localScale;
        var font = widget.textName.font;

        LayoutObj = new GameObject("MCSEDHealthBar", typeof(LayoutElement));
        Layout = LayoutObj.GetComponent<LayoutElement>();
        LayoutObj.transform.SetParent(widget.layout.transform);
        LayoutObj.transform.localScale = localScale;

        BGImage = AddHealthBarImage(Layout, "MCSEDHealthBarBG", localScale, Config.HealthBarBGColor);
        FGDamageImage = AddHealthBarImage(Layout, "MCSEDHealthBarFGDamege", localScale, Config.HealthBarBGColor);
        FGImage = AddHealthBarImage(Layout, "MCSEDHealthBarFG", localScale, Config.HealthBarFGColor);

        var valueObj = new GameObject("MCSEDHealthBarValue", typeof(UIText), typeof(Shadow));
        ValueText = valueObj.GetComponent<UIText>();
        ValueText.enabled = ProfileConfig.HealthBar.DisplayValue;
        ValueText.supportRichText = true;
        ValueText.font = font;
        ValueText.text = string.Empty;
        ValueText.alignment = TextAnchor.MiddleCenter;
        valueObj.transform.SetParent(Layout.transform);
        valueObj.transform.localScale = localScale;

        var valueShadow = valueObj.GetComponent<Shadow>();
        valueShadow.effectColor = new Color(0, 0, 0);
        valueShadow.effectDistance = new Vector2(1, -1);
        valueShadow.useGraphicAlpha = true;
    }

    private static Texture2D CreateTexture(Color32 color)
    {
        var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        var pixelData = texture.GetPixelData<Color32>(0);
        for (var i = 0; i < pixelData.Length; i++)
        {
            pixelData[i] = color;
        }
        texture.Apply();

        return texture;
    }

    private static UIImage AddHealthBarImage(LayoutElement layout, string name, Vector3 localScale, Color color)
    {
        // GameObjectを生成し、layoutに挿入する
        var obj = new GameObject(name, typeof(UIImage));
        // 体力バーの画像を設定する
        var image = obj.GetComponent<UIImage>();
        image.sprite = Sprite.Create(BarTexture, new Rect(0, 0, BarTexture.width, BarTexture.height), Vector2.zero);
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
        var pctColor = Color.Lerp(Config.HealthBarLowValueTextColor, Config.HealthBarValueTextColor, ratio);
        var barColor = Color.Lerp(Config.HealthBarLowValueFGColor, Config.HealthBarFGColor, ratio);
        var pctText = pct == 0 || pct >= 100 ? $"{pct:0}" : $"{pct:0.0}";

        ValueText.text = $"{pctText}%".TagColor(pctColor);
        FGImage.fillAmount = ratio;
        FGImage.color = barColor;
        // 体力バーの減少をアニメーションで表現する
        if (Target == chara && Layout.IsActive() && FGDamageImage.fillAmount > ratio)
        {
            FGDamageImage.color = Config.HealthBarFGDamageColor;
            // フェードアウト時に体力バーの減少を表現するための画像が目立たないようにする
            DamageTween = FGDamageImage
                .DOFillAmount(ratio, (FGDamageImage.fillAmount - ratio) * 3)
                .SetDelay(0.1f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (DamageTween is null || !DamageTween.IsPlaying())
                    {
                        FGDamageImage.color = Config.HealthBarBGColor;
                    }
                });
        }
        else
        {
            // 対象が変わった場合も体力の割合が変わりうるため、
            // 体力バーのアニメーションを行わないようにする
            DamageTween?.Kill(true);
            FGDamageImage.fillAmount = ratio;
        }

        Target = chara;
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
            FGDamageImage.enabled = value;
            FGImage.enabled = value;
            ValueText.enabled = value && ProfileConfig.HealthBar.DisplayValue;
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
            width = ProfileConfig.HealthBar.Width * sizeRatio;
            height = Height * sizeRatio;
            barHeight = BarHeight * sizeRatio;
        }

        UpdateTransformSize(Layout, width, height);
        UpdateTransformSize(BGImage, width, barHeight);
        UpdateTransformSize(FGDamageImage, width, barHeight);
        UpdateTransformSize(FGImage, width, barHeight);
        UpdateTransformSize(ValueText, width, height);
    }
}

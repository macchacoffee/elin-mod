using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SomewhatEnhancedDisplay.UI;

public class ModHealthBar
{
    private static readonly Color BGColor = new Color(0.2f, 0.1f, 0.1f);
    private static readonly Color FGDamageColor = new Color(0.6f, 0.6f, 0.6f);
    private static readonly Color FGColor = new Color(0.212f, 0.459f, 0.184f);
    private static readonly Color LowValueFGColor = new Color(0.485f, 0.189f, 0.104f);
    private static readonly Color LowValueTextColor = new(0.872f, 0.371f, 0.335f);
    private static readonly float Height = 24;
    private static readonly float Width = 400;
    private static readonly float BarHeight = 8;

    private static readonly Texture2D BarTexture = CreateTexture(Color.white);

    private GameObject LayoutObj { get; }
    private LayoutElement Layout { get; }
    private UIImage BGImage { get; }
    private UIImage FGImage { get; }
    private UIImage FGDamageImage { get; }
    private UIText ValueText { get; }
    private Color ValueTextColor { get; }

    private Chara? Target { get; set; }
    private Tween? DamageTween { get; set; }

    public ModHealthBar(WidgetMouseover widget)
    {
        var localScale = widget.textName.transform.localScale;
        var font = widget.textName.font;
        ValueTextColor = widget.textName.color;

        LayoutObj = new GameObject("MCSEDHealthBar", typeof(LayoutElement));
        Layout = LayoutObj.GetComponent<LayoutElement>();
        Layout.transform.Rect().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Width);
        Layout.transform.Rect().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Height);
        LayoutObj.transform.SetParent(widget.layout.transform);
        LayoutObj.transform.localScale = localScale;

        BGImage = AddHealthBarImage(Layout, "MCSEDHealthBarBG", Width, BarHeight, localScale, BGColor);
        FGDamageImage = AddHealthBarImage(Layout, "MCSEDHealthBarFGDamege", Width, BarHeight, localScale, BGColor);
        FGImage = AddHealthBarImage(Layout, "MCSEDHealthBarFG", Width, BarHeight, localScale, FGColor);

        var valueObj = new GameObject("MCSEDHealthBarValue", typeof(UIText), typeof(Shadow));
        // 体力バー背景の画像を設定する
        ValueText = valueObj.GetComponent<UIText>();
        ValueText.supportRichText = true;
        ValueText.font = font;
        ValueText.fontSize = 16;
        ValueText.color = ValueTextColor;
        ValueText.text = string.Empty;
        ValueText.transform.Rect().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Width);
        ValueText.transform.Rect().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Height);
        ValueText.alignment = TextAnchor.MiddleCenter;

        var valueShadow = valueObj.GetComponent<Shadow>();
        valueShadow.effectColor = new Color(0, 0, 0);
        valueShadow.effectDistance = new Vector2(1, -1);
        valueShadow.useGraphicAlpha = true;
        valueObj.transform.SetParent(Layout.transform);
        valueObj.transform.localScale = localScale;
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

    private static UIImage AddHealthBarImage(LayoutElement layout, string name, float width, float height, Vector3 localScale, Color color)
    {
        // GameObjectを生成し、layoutに挿入する
        var obj = new GameObject(name, typeof(UIImage));
        // 体力バーの画像を設定する
        var image = obj.GetComponent<UIImage>();
        image.sprite = Sprite.Create(BarTexture, new Rect(0, 0, BarTexture.width, BarTexture.height), Vector2.zero);
        image.color = color;
        image.transform.Rect().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        image.transform.Rect().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        image.type = Image.Type.Filled;
        image.fillOrigin = (int)Image.OriginHorizontal.Left;
        image.fillMethod = Image.FillMethod.Horizontal;
        obj.transform.SetParent(layout.transform);
        obj.transform.localScale = localScale;

        return image;
    }

    public void Update(Chara chara)
    {
        var ratio = GetHealthRatio(chara);
        // 0%または100%以上の場合は小数点以下なし、それ以外の場合は小数第1位まで表示する
        // 現在HPが最大HPよりも1でも低ければ100%とは表示しないようにする
        var ceilingRatio = Math.Ceiling(ratio * 1000);
        var pct =(float)(ratio < 1 ? Math.Min(ceilingRatio, 999) : ceilingRatio) / 10;
        var pctColor = Color.Lerp(LowValueTextColor, ValueTextColor, ratio);
        var barColor = Color.Lerp(LowValueFGColor, FGColor, ratio);
        var pctText = pct == 0 || pct >= 100 ? $"{pct:0}" : $"{pct:0.0}";

        ValueText.text = $"{pctText}%".TagColor(pctColor);
        FGImage.fillAmount = ratio;
        FGImage.color = barColor;
        // 体力バーの減少をアニメーションで表現する
        if (Target == chara && FGDamageImage.fillAmount > ratio)
        {
            FGDamageImage.color = FGDamageColor;
            // フェードアウト時に体力バーの減少を表現するための画像が目立たないようにする
            DamageTween = FGDamageImage
                .DOFillAmount(ratio, (FGDamageImage.fillAmount - ratio) * 4)
                .SetDelay(0.1f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (DamageTween is null || !DamageTween.IsPlaying()) {
                        FGDamageImage.color = BGColor;
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

    public void SetActive(bool value)
    {
        LayoutObj.SetActive(value);
    }

    private static float GetHealthRatio(Chara chara)
    {
        var health = Math.Max(chara.hp, 0);
        var maxHealth = Math.Max(chara.MaxHP, 0);
        if (chara.HasElement(FEAT.featManaMeat))
        {
            // マナの体フィートを持っている場合はマナも体力の一部として扱う
            health += Math.Max(chara.mana.value, 0);
            maxHealth += Math.Max(chara.mana.max, 0);
        }
        return (float)health / maxHealth;
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

namespace SomewhatEnhancedDisplay.UI;

public class ModHealthBar
{
    private static readonly Color BGColor = new Color(0.1f, 0, 0);
    private static readonly Color FGColor = new Color(0.212f, 0.459f, 0.184f);
    private static readonly Color LowValueTextColor = new(0.872f, 0.371f, 0.335f);

    private static readonly Texture2D FGTexutre;
    private static readonly Texture2D BGTexture;

    private GameObject LayoutObj { get; }
    private LayoutElement Layout { get; }
    private UIImage BGImage { get; }
    private UIImage FGImage { get; }
    private UIText ValueText { get; }

    static ModHealthBar()
    {
        BGTexture = CreateTexture(BGColor);
        FGTexutre = CreateTexture(FGColor);
    }

    public ModHealthBar(WidgetMouseover widget)
    {
        LayoutObj =  new GameObject("MCSEDHealth", typeof(LayoutElement));
        Layout = LayoutObj.GetComponent<LayoutElement>();
        Layout.transform.Rect().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400);
        Layout.transform.Rect().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 24);
        LayoutObj.transform.SetParent(widget.layout.transform);
        LayoutObj.transform.localScale = widget.textName.transform.localScale;

        // GameObjectを生成し、ウィジェットのLayoutGroupに挿入する
        var bgObj = new GameObject("MCSEDHealthBG", typeof(UIImage));
        // 体力バー背景の画像を設定する
        BGImage = bgObj.GetComponent<UIImage>();
        BGImage.sprite = Sprite.Create(BGTexture, new Rect(0, 0, BGTexture.width, BGTexture.height), Vector2.zero);
        BGImage.transform.Rect().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400);
        BGImage.transform.Rect().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 12);
        BGImage.type = Image.Type.Filled;
        BGImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        BGImage.fillMethod = Image.FillMethod.Horizontal;
        bgObj.transform.SetParent(Layout.transform);
        bgObj.transform.localScale = widget.textName.transform.localScale;

        // GameObjectを生成し、ウィジェットのLayoutGroupに挿入する
        var fgObj = new GameObject("MCSEDHealthBarFG", typeof(UIImage));
        // 体力バーの画像を設定する
        FGImage = fgObj.GetComponent<UIImage>();
        FGImage.sprite = Sprite.Create(FGTexutre, new Rect(0, 0, FGTexutre.width, FGTexutre.height), Vector2.zero);
        FGImage.transform.Rect().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400);
        FGImage.transform.Rect().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 12);
        FGImage.type = Image.Type.Filled;
        FGImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        FGImage.fillMethod = Image.FillMethod.Horizontal;
        fgObj.transform.SetParent(Layout.transform);
        fgObj.transform.localScale = widget.textName.transform.localScale;

        var valueObj = new GameObject("MCSEDHealthBarValue", typeof(UIText), typeof(Shadow));
        // 体力バー背景の画像を設定する
        ValueText = valueObj.GetComponent<UIText>();
        ValueText.supportRichText = true;
        ValueText.font = widget.textName.font;
        ValueText.fontSize = 16;
        ValueText.text = "100%";
        ValueText.transform.Rect().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400);
        ValueText.transform.Rect().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30);
        ValueText.alignment = TextAnchor.MiddleCenter;

        var valueShadow = valueObj.GetComponent<Shadow>();
        valueShadow.effectColor = new Color(0, 0, 0);
        valueShadow.effectDistance = new Vector2(1, -1);
        valueShadow.useGraphicAlpha = true;
        valueObj.transform.SetParent(Layout.transform);
        valueObj.transform.localScale = widget.textName.transform.localScale;
    }

    public void Update(Chara chara)
    {
        var pct = GetHealthPercentage(chara);
        var ratio = (float)(pct / 100);

        var pctColor = Color.Lerp(LowValueTextColor, Color.white, (float)(pct / 100));
        var pctText = pct == 0 || pct >= 100 ?$"{pct:0}" : $"{pct:0.0}";
        ValueText.text = $"{pctText}%".TagColor(pctColor);
        FGImage.fillAmount = ratio; 
    }

    public void SetActive(bool value)
    {
        LayoutObj.SetActive(value);
    }

    private static float GetHealthPercentage(Chara chara)
    {
        var health = Math.Max(chara.hp, 0);
        var maxHealth = Math.Max(chara.MaxHP, 0);
        if (chara.HasElement(FEAT.featManaMeat))
        {
            // マナの体フィートを持っている場合はマナも体力の一部として扱う
            health += Math.Max(chara.mana.value, 0);
            maxHealth += Math.Max(chara.mana.max, 0);
        }
        // 0%または100%以上の場合は小数点以下なし、それ以外の場合は小数第1位まで表示する
        // 現在HPが最大HPよりも1でも低ければ100%とは表示しないようにする
        var ratio = Math.Ceiling((float)health / maxHealth * 1000);
        return (float)((health < maxHealth ? Math.Min(ratio, 999) : ratio) / 10);
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
}

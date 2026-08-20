using UnityEngine;

namespace Macchacoffee.ElinMods.SomewhatEnhancedDisplay.UI;

internal static class ModUIUtil
{
    public static readonly Texture2D White1x1Texture = Create1x1WhiteTexture();
    public static readonly Sprite White1x1Sprite = Sprite.Create(
        White1x1Texture,
        new Rect(0, 0, White1x1Texture.width, White1x1Texture.height),
        Vector2.zero);

    public static int ComputeFontSize(int baseSize)
    {
        var gameConfigSize = EClass.core.config.font.fontWidget.size;
        return (int)((baseSize + gameConfigSize) * ModContext.WorldConfig.HoverGuide.ZoomScale);
    }

    public static float ComputeFontSize(float baseSize)
    {
        var gameConfigSize = EClass.core.config.font.fontWidget.size;
        return (baseSize + gameConfigSize) * ModContext.WorldConfig.HoverGuide.ZoomScale;
    }

    public static Texture2D Create1x1WhiteTexture()
    {
        return Create1x1ColorTexture(Color.white);
    }

    public static Texture2D Create1x1ColorTexture(Color color)
    {
        return CreateColorTexture(color, 1, 1);
    }

    public static Texture2D CreateColorTexture(Color color, int width, int height)
    {
        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        var pixelData = texture.GetPixelData<Color32>(0);
        for (var i = 0; i < pixelData.Length; i++)
        {
            pixelData[i] = color;
        }
        texture.Apply();

        return texture;
    }
}

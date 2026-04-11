namespace SomewhatEnhancedDisplay.UI;

public static class ModUIUtil
{
    public static int ComputeFontSize(int baseSize)
    {
        var gameConfigSize = EClass.core.config.font.fontWidget.size;
        return (int)((baseSize + gameConfigSize) * Mod.Config.HoverGuide.Scale);
    }

    public static float ComputeFontSize(float baseSize)
    {
        var gameConfigSize = EClass.core.config.font.fontWidget.size;
        return (baseSize + gameConfigSize) * Mod.Config.HoverGuide.Scale;
    }
}
using SomewhatEnhancedDisplay.Config;
using UnityEngine;

namespace SomewhatEnhancedDisplay.UI.HoverGuide;

public class ModHoverGuide
{
    private static readonly float PaddingHeight = 21;

    private ModHoverGuideItem Item1 { get; }
    private ModHoverGuidePadding Padding1 { get; }
    private ModHoverGuideItem Item2 { get; }

    private Vector2 OriginalPivot { get; }
    private int BaseFontSize { get; }

    private static ModConfigHoverGuide Config => Mod.Config.HoverGuide;

    public ModHoverGuide(WidgetMouseover widget)
    {
        Item1 = new(widget);
        Padding1 = new(widget);
        Item2 = new(widget);

        OriginalPivot = widget.layout.Rect().pivot;
        // ゲーム設定のウィジェットのフォントサイズが "すごく小さい" (最小値) の場合を基準のフォントサイズとする
        // OriginalFontSize = widget.textName.fontSize;
        BaseFontSize = widget.textName.fontSize - EClass.core.config.font.fontWidget.size;

        // ウィジェットを無効から有効に切り替えた際に表示が乱れないようにするため、
        // 初期状態では追加コンポーネントなどは表示しないようにする
        Item1.Enabled = false;
        Padding1.Enabled = false;
        Item2.Enabled = false;
    }

    public void Show(WidgetMouseover widget, ModHoverGuideTarget? target1, ModHoverGuideTarget? target2)
    {
        var fontColor = widget.textName.fontColor;
        var fontSize = ModUIUtil.ComputeFontSize(BaseFontSize);
        var paddingHeight = PaddingHeight * fontSize / BaseFontSize;

        var isItem1Enabled = Item1.Show(fontColor, fontSize, BaseFontSize, target1);
        var isItem2Enabled = Item2.Show(fontColor, fontSize, BaseFontSize, target2);
        Padding1.Update(isItem1Enabled && isItem2Enabled, paddingHeight);

        widget.textName.enabled = false;
        widget.Show(string.Empty);
        widget.layout.Rect().pivot = new(Config.HorizontalPivot, Config.VerticalPivot);
    }

    public void ShowForManager(WidgetMouseover widget)
    {
        widget.layout.Rect().pivot = OriginalPivot;
        widget.textName.enabled = true;

        Item1.ShowForManager();
        Padding1.Enabled = false;
        Item2.ShowForManager();

        widget.layout.RebuildLayout();
    }
}

using SomewhatEnhancedDisplay.Config;
using UnityEngine;

namespace SomewhatEnhancedDisplay.UI.HoverGuide;

public class ModHoverGuide
{
    private static readonly float PaddingHeight = 21;

    // 固定表示用
    private ModHoverGuideItem ItemExtra1 { get; }
    private ModHoverGuidePadding Padding1 { get; }
    private ModHoverGuideItem Item1 { get; }
    // 騎乗、寄生などの複数表示用
    private ModHoverGuidePadding Padding2 { get; }
    private ModHoverGuideItem Item2 { get; }

    private Vector2 OriginalPivot { get; }
    private int BaseFontSize { get; }

    private static ModConfigHoverGuide Config => Mod.Config.HoverGuide;

    public ModHoverGuide(WidgetMouseover widget)
    {
        ItemExtra1 = new(widget, prepends: true);
        Padding1 = new(widget);
        Item1 = new(widget);
        Padding2 = new(widget);
        Item2 = new(widget);

        OriginalPivot = widget.layout.Rect().pivot;
        // ゲーム設定のウィジェットのフォントサイズが "すごく小さい" (最小値) の場合を基準のフォントサイズとする
        // OriginalFontSize = widget.textName.fontSize;
        BaseFontSize = widget.textName.fontSize - EClass.core.config.font.fontWidget.size;

        // ウィジェットを無効から有効に切り替えた際に表示が乱れないようにするため、
        // 初期状態では追加コンポーネントなどは表示しないようにする
        ItemExtra1.Enabled = false;
        Padding1.Enabled = false;
        Item1.Enabled = false;
        Padding2.Enabled = false;
        Item2.Enabled = false;
    }

    public void Show(WidgetMouseover widget, ModHoverGuideTarget? target1, ModHoverGuideTarget? target2, ModHoverGuideTarget? targetExtra)
    {
        var fontColor = widget.textName.fontColor;
        var fontSize = ModUIUtil.ComputeFontSize(BaseFontSize);
        var paddingHeight = PaddingHeight * fontSize / BaseFontSize;

        var isItemExtra1Enabled = ItemExtra1.Show(fontColor, fontSize, BaseFontSize, targetExtra);
        var isItem1Enabled = Item1.Show(fontColor, fontSize, BaseFontSize, target1);
        Padding1.Update(isItemExtra1Enabled && isItem1Enabled, paddingHeight);
        var isItem2Enabled = Item2.Show(fontColor, fontSize, BaseFontSize, target2);
        Padding2.Update(isItem1Enabled && isItem2Enabled, paddingHeight);

        widget.textName.enabled = false;
        widget.Show("");
        widget.layout.Rect().pivot = new(Config.HorizontalPivot, Config.VerticalPivot);
    }

    public void ShowForManager(WidgetMouseover widget)
    {
        widget.layout.Rect().pivot = OriginalPivot;
        widget.textName.enabled = true;

        ItemExtra1.ShowForManager();
        Padding1.Enabled = false;
        Item1.ShowForManager();
        Padding2.Enabled = false;
        Item2.ShowForManager();

        widget.layout.RebuildLayout();
    }
}

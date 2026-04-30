using System;
using SomewhatEnhancedDisplay.Config;
using UnityEngine;

namespace SomewhatEnhancedDisplay.UI.HoverGuide;

public class ModHoverGuide
{
    private static readonly float PaddingHeight = 24;

    private ModHoverGuideItem Item1 { get; }
    private ModHoverGuidePadding Padding1 { get; }
    private ModHoverGuideItem Item2 { get; }

    private Vector2 OriginalPivot { get; }
    private int BaseFontSize { get; }

    public bool LocksCard { get; set; } = false;
    private WeakReference<Card?> LockedCard { get; set; } = new(null);
    private ModHoverGuideTargetModifier? LockedModifier { get; set; }

    private static ModConfigHoverGuide Config => Mod.Config.HoverGuide;

    public ModHoverGuide(WidgetMouseover widget)
    {
        Item1 = new(widget);
        Padding1 = new(widget);
        Item2 = new(widget);

        OriginalPivot = widget.layout.Rect().pivot;
        // ゲーム設定のウィジェットのフォントサイズが "すごく小さい" (最小値) の場合を基準のフォントサイズとする
        BaseFontSize = widget.textName.fontSize - EClass.core.config.font.fontWidget.size;

        // ウィジェットを無効から有効に切り替えた際に表示が乱れないようにするため、
        // 初期状態では追加コンポーネントなどは表示しないようにする
        Item1.Enabled = false;
        Padding1.Enabled = false;
        Item2.Enabled = false;
    }

    private Card? GetOrUpdateLockedCard()
    {
        return GetOrUpdateLockedCard(null);
    }

    private Card? GetOrUpdateLockedCard(Card? newCard)
    {
        Card? lockedCard = null;
        if (LocksCard && !LockedCard.TryGetTarget(out lockedCard))
        {
            if (newCard is Card card)
            {
                LockedCard.SetTarget(card);
                lockedCard = card;
            }
        }
        else if (!LocksCard || (lockedCard is Card card && !card.ExistsOnMap))
        {
            UnlockCard();
            lockedCard = null;
        }

        return lockedCard;
    }

    private static bool IsWidgetActive(WidgetMouseover widget)
    {
        return widget.config.state == Widget.State.Active;
    }

    public void LockCard(Card card)
    {
        LockCard(card, null);
    }

    public void LockCard(Card card, ModHoverGuideTargetModifier? modifier)
    {
        LockedCard.SetTarget(card);
        LockedModifier = modifier;
        LocksCard = true;
    }

    public void UnlockCard()
    {
        LockedCard.SetTarget(null);
        LockedModifier = null;
        LocksCard = false;
    }

    public void Show(WidgetMouseover? widget, ModHoverGuideTarget? target1, ModHoverGuideTarget? target2)
    {
        if (widget is null || !IsWidgetActive(widget))
        {
            return;
        }

        var card1 = target1?.Card;
        var lockedCard = GetOrUpdateLockedCard(card1);
        if (lockedCard is not null)
        {
            if (lockedCard != card1)
            {
                target1 = new ModHoverGuideTarget(lockedCard.GetHoverText(), lockedCard.GetHoverText2(), lockedCard, LockedModifier);
            }
            target2 = null;
        }

        ShowInternal(widget, target1, target2, lockedCard is not null);
    }

    public bool TryShowLockedCard(WidgetMouseover? widget)
    {
        if (widget is null || !IsWidgetActive(widget))
        {
            return false;
        }
        if (GetOrUpdateLockedCard() is not Card card)
        {
            return false;
        }

        var target = new ModHoverGuideTarget(card.GetHoverText(), card.GetHoverText2(), card, LockedModifier);
        ShowInternal(widget, target, null, true);

        return true;
    }

    private void ShowInternal(WidgetMouseover widget, ModHoverGuideTarget? target1, ModHoverGuideTarget? target2, bool isLocked)
    {
        var fontColor = widget.textName.fontColor;
        // 行間を広げるためにフォントサイズを少し大きく設定する
        var fontSize1 = ModUIUtil.ComputeFontSize(BaseFontSize + 2);
        var fontSize2 = ModUIUtil.ComputeFontSize(BaseFontSize + 4);
        var sizeRatio = (float)fontSize2 / BaseFontSize;
        var paddingHeight = PaddingHeight * sizeRatio;

        var isItem1Enabled = Item1.Show(fontColor, fontSize1, target1, isLocked);
        var isItem2Enabled = Item2.Show(fontColor, fontSize2, target2, false);
        Padding1.Update(isItem1Enabled && isItem2Enabled, paddingHeight);

        widget.textName.enabled = false;
        widget.Show(string.Empty);
        widget.layout.Rect().pivot = new(Config.HorizontalPivot, Config.VerticalPivot);
    }

    public void ShowForManager(WidgetMouseover? widget)
    {
        if (widget is null || !IsWidgetActive(widget))
        {
            return;
        }

        widget.layout.Rect().pivot = OriginalPivot;
        widget.textName.enabled = true;

        Item1.ShowForManager();
        Padding1.Enabled = false;
        Item2.ShowForManager();

        widget.layout.RebuildLayout();
    }
}

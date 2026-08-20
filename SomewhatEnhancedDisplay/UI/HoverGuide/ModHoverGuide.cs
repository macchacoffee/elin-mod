using System;

using UnityEngine;

using Macchacoffee.ElinMods.SomewhatEnhancedDisplay.Config;

namespace Macchacoffee.ElinMods.SomewhatEnhancedDisplay.UI.HoverGuide;

internal class ModHoverGuide
{
    private const float _paddingHeight = 24;

    private ModHoverGuideItem Item1 { get; }
    private ModHoverGuidePadding Padding1 { get; }
    private ModHoverGuideItem Item2 { get; }

    private Vector2 OriginalPivot { get; }
    private int BaseFontSize { get; }

    public bool LocksCard { get; private set; } = false;
    private WeakReference<Card?> LockedCard { get; set; } = new(null);
    private ModHoverGuideTargetModifier? LockedModifier { get; set; }
    private WeakReference<Card?> LockCandidateCard { get; } = new(null);
    private ModHoverGuideTargetModifier? LockCandidateModifier { get; set; }

    private WidgetMouseover MouseoverWidget { get; }

    private static ModConfigHoverGuide Config => ModContext.WorldConfig.HoverGuide;

    public ModHoverGuide(WidgetMouseover widget)
    {
        MouseoverWidget = widget;

        Item1 = new(widget);
        Padding1 = new(widget);
        Item2 = new(widget);

        OriginalPivot = widget.layout.Rect().pivot;
        // ゲーム設定のウィジェットのフォントサイズが "すごく小さい" (最小値) の場合を基準のフォントサイズとする。
        BaseFontSize = widget.textName.fontSize - EClass.core.config.font.fontWidget.size;

        // ウィジェットを無効から有効に切り替えた際に表示が乱れないようにするため、
        // 初期状態では追加コンポーネントなどは表示しないようにする。
        Padding1.Enabled = false;
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
        else if (!LocksCard)
        {
            UnlockCard();
            lockedCard = null;
        }
        else if (lockedCard is Card card && !card.ExistsOnMap)
        {
             UnlockCard();
             ClearTarget();
             lockedCard = null;
        }

        return lockedCard;
    }

    private bool IsWidgetActive()
    {
        return MouseoverWidget.config.state == Widget.State.Active;
    }

    private bool IsVisible()
    {
        // widgetが非可視になるのはlayoutが非活性になるパターンと透明度が非常に小さい値になるパターンがある。
        return MouseoverWidget.layout.isActiveAndEnabled && MouseoverWidget.cg.alpha >= 0.07f;
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

    private void ClearLockCandidate()
    {
        LockCandidateCard.SetTarget(null);
        LockCandidateModifier = null;
    }

    public bool TryShowLockedCard()
    {
        if (!IsWidgetActive())
        {
            return false;
        }
        if (GetOrUpdateLockedCard() is not Card card)
        {
            return false;
        }

        var target = CreateLockedTarget(card);
        ShowInternal(target, null, true);

        return true;
    }

    public bool TryToggleLock()
    {
        if (LocksCard)
        {
            UnlockCard();
            return true;
        }

        if (!IsWidgetActive()
            || !IsVisible()
            || !LockCandidateCard.TryGetTarget(out var card)
            || card is null
            || !card.ExistsOnMap)
        {
            return false;
        }

        LockCard(card, LockCandidateModifier);

        var target = CreateLockedTarget(card);
        ShowInternal(target, null, true);

        return true;
    }

    private ModHoverGuideTarget CreateLockedTarget(Card card)
    {
        return new ModHoverGuideTarget(card.GetHoverText(), card.GetHoverText2(), card, LockedModifier);
    }

    public void Show(ModHoverGuideTarget? target1, ModHoverGuideTarget? target2)
    {
        if (!IsWidgetActive())
        {
            return;
        }

        var card1 = target1?.Card;
        var lockedCard = GetOrUpdateLockedCard(card1);
        if (lockedCard is not null)
        {
            if (lockedCard != card1)
            {
                target1 = CreateLockedTarget(lockedCard);
            }
            target2 = null;
        }

        if (card1 is not null)
        {
            LockCandidateCard.SetTarget(card1);
            LockCandidateModifier = target1?.Modifier;
        }

        ShowInternal(target1, target2, lockedCard is not null);
    }

    private void ShowInternal(ModHoverGuideTarget? target1, ModHoverGuideTarget? target2, bool isLocked)
    {
        if (!IsVisible())
        {
            // 非可視の場合はホバーガイドのターゲットをクリアする。
            ClearTarget();
            if (!LocksCard)
            {
                ClearLockCandidate();
            }
        }

        var fontColor = MouseoverWidget.textName.fontColor;
        // 行間を広げるためにフォントサイズを少し大きく設定する。
        var fontSize1 = ModUIUtil.ComputeFontSize(BaseFontSize + 2);
        var fontSize2 = ModUIUtil.ComputeFontSize(BaseFontSize + 4);
        var sizeRatio = (float)fontSize2 / BaseFontSize;
        var paddingHeight = _paddingHeight * sizeRatio;

        var isItem1Enabled = Item1.Show(fontColor, fontSize1, target1, isLocked);
        var isItem2Enabled = Item2.Show(fontColor, fontSize2, target2, false);
        Padding1.Update(isItem1Enabled && isItem2Enabled, paddingHeight);

        MouseoverWidget.textName.enabled = false;
        MouseoverWidget.Show(string.Empty);
        MouseoverWidget.layout.childAlignment = TextAnchor.MiddleCenter;
        MouseoverWidget.layout.Rect().pivot = new(Config.HorizontalPivot, Config.VerticalPivot);
    }

    public void ShowForManager()
    {
        if (!IsWidgetActive())
        {
            return;
        }

         ClearLockCandidate();

        MouseoverWidget.layout.Rect().pivot = OriginalPivot;
        MouseoverWidget.textName.enabled = true;

        Item1.ShowForManager();
        Padding1.Enabled = false;
        Item2.ShowForManager();

        MouseoverWidget.layout.RebuildLayout();
    }

    public void UpdateHealthBars()
    {
        Item1.UpdateHealthBar();
        Item2.UpdateHealthBar();
    }

    public void ClearTarget()
    {
        Item1.ClearTarget();
        Item2.ClearTarget();
    }
}

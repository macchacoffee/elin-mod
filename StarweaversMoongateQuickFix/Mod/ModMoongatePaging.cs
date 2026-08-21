using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Macchacoffee.ElinMods.StarweaversMoongateQuickFix.Mod;

internal static class ModMoongatePaging
{
    public static bool IsOpening { get; set; }

    private static LayerList? _layer;
    private static ModPagedCollection<MapMetaData>? _maps;
    private static UIButton? _previousButton;
    private static UIButton? _pageIndicator;
    private static UIButton? _nextButton;

    public static void Attach(LayerList layer, List<MapMetaData> source, ref ICollection<MapMetaData> collection)
    {
        Release();

        var paged = new ModPagedCollection<MapMetaData>(source, ModContext.Config.ItemsPerPage.Value);
        _layer = layer;
        _maps = paged;
        collection = paged;
    }

    public static void SetupPageControls()
    {
        if (!_layer || _maps == null || _maps.PageCount <= 1 || _layer!.windows.Count == 0 ||
            _previousButton != null || _pageIndicator != null || _nextButton != null)
        {
            return;
        }

        var window = _layer.windows[0];
        _previousButton = window.AddBottomButton(ModConsts.SourceId.PreviousPage, PreviousPage, setFirst: true);
        _previousButton.soundClick = SE.DataClick;
        _nextButton = window.AddBottomButton(ModConsts.SourceId.NextPage, NextPage);
        _nextButton.soundClick = SE.DataClick;
        _pageIndicator = CreatePageIndicator(window, _previousButton.transform.GetSiblingIndex() + 1);
        _nextButton.transform.SetSiblingIndex(_pageIndicator.transform.GetSiblingIndex() + 1);
        window.rectBottom.RebuildLayout(recursive: true);

        UpdatePageControls();
    }

    private static UIButton CreatePageIndicator(Window window, int siblingIndex)
    {
        var indicator = window.AddBottomButton("", IgnorePageIndicatorClick);
        indicator.onClick.RemoveListener(IgnorePageIndicatorClick);
        indicator.transform.SetSiblingIndex(siblingIndex);
        indicator.soundClick = null;
        indicator.soundHighlight = null;
        var decoration = indicator.transform.Find("Image");
        if (decoration)
        {
            decoration.gameObject.SetActive(false);
        }

        var layoutGroup = indicator.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup)
        {
            var padding = layoutGroup.padding;
            var horizontalPadding = (padding.left + padding.right) / 2;
            layoutGroup.padding = new RectOffset(horizontalPadding, horizontalPadding, padding.top, padding.bottom);
        }

        var navigation = indicator.navigation;
        navigation.mode = Navigation.Mode.None;
        indicator.navigation = navigation;

        var canvasGroup = indicator.GetComponent<CanvasGroup>() ?? indicator.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
        indicator.RebuildLayout(recursive: true);
        return indicator;
    }

    private static void IgnorePageIndicatorClick()
    {
    }

    private static void NextPage()
    {
        if (!_layer || _maps == null || !_maps.MoveNext())
        {
            return;
        }

        Refresh();
    }

    private static void PreviousPage()
    {
        if (!_layer || _maps == null || !_maps.MovePrevious())
        {
            return;
        }

        Refresh();
    }

    private static void Refresh()
    {
        _layer!.list.List();
        _layer.RefreshSize();

        if (_layer.scroll)
        {
            _layer.scroll.verticalNormalizedPosition = 1f;
        }
    }

    public static void UpdatePageControls(UIList list)
    {
        if (!_layer || _layer!.list != list)
        {
            return;
        }

        UpdatePageControls();
    }

    private static void UpdatePageControls()
    {
        var previousButton = _previousButton;
        var pageIndicator = _pageIndicator;
        var nextButton = _nextButton;
        if (!_layer || _maps == null || previousButton == null || pageIndicator == null || nextButton == null)
        {
            return;
        }

        _maps.ClampPageIndex();

        var showControls = _maps.PageCount > 1;
        previousButton.SetActive(showControls);
        pageIndicator.SetActive(showControls);
        nextButton.SetActive(showControls);
        if (!showControls)
        {
            return;
        }

        pageIndicator.mainText.SetText($"{_maps.PageIndex + 1} / {_maps.PageCount}");
        pageIndicator.RebuildLayout(recursive: true);
        previousButton.SetInteractableWithAlpha(_maps.CanMovePrevious);
        nextButton.SetInteractableWithAlpha(_maps.CanMoveNext);
    }

    public static void Detach(LayerList layer)
    {
        if (_layer != layer)
        {
            return;
        }

        Release();
    }

    public static void AbortOpening()
    {
        Release();
    }

    private static void Release()
    {
        if (_previousButton != null)
        {
            _previousButton.onClick.RemoveListener(PreviousPage);
            _previousButton.SetActive(false);
        }
        if (_nextButton != null)
        {
            _nextButton.onClick.RemoveListener(NextPage);
            _nextButton.SetActive(false);
        }
        if (_pageIndicator != null)
        {
            _pageIndicator.SetActive(false);
        }

        _layer = null;
        _maps = null;
        _previousButton = null;
        _pageIndicator = null;
        _nextButton = null;
    }
}

using System.Collections.Generic;

namespace Macchacoffee.ElinMods.StarweaversMoongateQuickFix.Mod;

internal static class ModMoongatePaging
{
    public static bool IsOpening { get; set; }

    private static LayerList? _layer;
    private static ModPagedCollection<MapMetaData>? _maps;
    private static UIButton? _previousButton;
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
            _previousButton != null || _nextButton != null)
        {
            return;
        }

        var window = _layer.windows[0];
        _previousButton = window.AddBottomButton("", PreviousPage, setFirst: true);
        _nextButton = window.AddBottomButton("", NextPage);

        UpdatePageControls();
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
        var nextButton = _nextButton;
        if (!_layer || _maps == null || previousButton == null || nextButton == null)
        {
            return;
        }

        _maps.ClampPageIndex();

        var showControls = _maps.PageCount > 1;
        previousButton.SetActive(showControls);
        nextButton.SetActive(showControls);
        if (!showControls)
        {
            return;
        }

        var pageText = $"{_maps.PageIndex + 1} / {_maps.PageCount}";
        previousButton.mainText.text = $"◀  {pageText}";
        nextButton.mainText.text = $"{pageText}  ▶";
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

        _layer = null;
        _maps = null;
        _previousButton = null;
        _nextButton = null;
    }
}

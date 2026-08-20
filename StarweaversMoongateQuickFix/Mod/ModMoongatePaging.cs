using System.Collections.Generic;

namespace Macchacoffee.ElinMods.StarweaversMoongateQuickFix.Mod;

internal static class ModMoongatePaging
{
    private const int PageSize = 50;

    public static bool IsOpening { get; set; }

    private static LayerList? _layer;
    private static ModPagedCollection<MapMetaData>? _maps;

    public static void Attach(LayerList layer, List<MapMetaData> source, ref ICollection<MapMetaData> collection)
    {
        var paged = new ModPagedCollection<MapMetaData>(source, PageSize);
        _layer = layer;
        _maps = paged;
        collection = paged;
    }

    public static void SetupPageButton()
    {
        if (!_layer || _maps == null || !_layer!.buttonReroll)
        {
            return;
        }

        var button = _layer.buttonReroll;

        button.SetActive(true);
        button.onClick.RemoveAllListeners();

        // 左クリック: 次ページ
        button.onClick.AddListener(NextPage);
        // 右クリック: 前ページ
        button.onRightClick = PrevPage;

        UpdatePageButton();
    }

    public static void NextPage()
    {
        if (!_layer || _maps == null)
        {
            return;
        }

        _maps.NextPage();
        Refresh();
    }

    public static void PrevPage()
    {
        if (!_layer || _maps == null)
        {
            return;
        }

        _maps.PrevPage();
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

        UpdatePageButton();
    }

    public static void UpdatePageButton()
    {
        if (!_layer || _maps == null || !_layer!.buttonReroll)
        {
            return;
        }

        _maps.NormalizePage();

        var button = _layer.buttonReroll;
        if (button.mainText)
        {
            button.mainText.text = $"{_maps.Page + 1} / {_maps.PageCount}";
        }
    }

    public static void Detach(LayerList layer)
    {
        if (_layer != layer)
        {
            return;
        }

        _layer = null;
        _maps = null;
    }
}

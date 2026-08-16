using UnityEngine;

namespace SomewhatEnhancedDisplay.UI.HoverGuide;

internal class ModHoverGuidePadding
{
    private UIImage Image { get; }

    private float Height { get; set; }
    private float _actualHeight = float.NaN;

    public bool Enabled
    {
        get => Image.enabled;
        set => Update(value, Height);
    }

    public ModHoverGuidePadding(WidgetMouseover widget)
    {
        var localScale = widget.textName.transform.localScale;

        // GameObjectを生成し、layoutに挿入する。
        var obj = new GameObject(ModConsts.GameObjectName.HoverGuidePadding);
        Image = obj.AddComponent<UIImage>();
        Image.transform.Rect().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        obj.transform.SetParent(widget.layout.transform);
        obj.transform.localScale = localScale;

        // ウィジェットを無効から有効に切り替えた際に表示が乱れないようにするため、
        // 初期状態では追加コンポーネントなどは表示しないようにする。
        Update(false, 0);
    }

    public void Update(bool enabled, float height)
    {
        Height = height;

        if (Image.enabled != enabled)
        {
             Image.enabled = enabled;
        }

        var actualHeight = enabled ? height : 0;
        if (_actualHeight == actualHeight)
        {
            return;
        }
        _actualHeight = actualHeight;

        Image.transform.Rect().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, actualHeight);
    }
}

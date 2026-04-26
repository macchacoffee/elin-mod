using UnityEngine;

namespace SomewhatEnhancedDisplay.UI.HoverGuide;

public class ModHoverGuidePadding
{
    public UIImage Image { get; }

    private float Height { get; set; }

    public bool Enabled
    {
        get
        {
            return Image.enabled;
        }
        set
        {
            Image.enabled = value;
            Image.transform.Rect().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value ? Height : 0);
        }
    }

    public ModHoverGuidePadding(WidgetMouseover widget)
    {
        var localScale = widget.textName.transform.localScale;

        // GameObjectを生成し、layoutに挿入する
        var obj = new GameObject(ModConsts.GameObjectName.HoverGuidePadding);
        Image = obj.AddComponent<UIImage>();
        Image.transform.Rect().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        obj.transform.SetParent(widget.layout.transform);
        obj.transform.localScale = localScale;

        // ウィジェットを無効から有効に切り替えた際に表示が乱れないようにするため、
        // 初期状態では追加コンポーネントなどは表示しないようにする
        Height = 0;
        Update(false, Height);
    }

    public void Update(bool enabled, float height)
    {
        Height = height;
        Enabled = enabled;
    }
}

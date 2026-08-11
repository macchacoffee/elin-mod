using UnityEngine;
using YKF;

namespace NoPCC.UI;

public class LayerModConfig : YKLayer<object>
{
    public override string Title { get; } = ModConsts.SourceId.ModName;
    public override Rect Bound { get; } = new Rect(0, 0, 440, 440);

    public override void OnLayout()
    {
        CreateTab<ModConfigMainTab>(ModConsts.SourceId.ModName, "nopcc.config.main");
    }
}

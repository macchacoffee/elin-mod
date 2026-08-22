using UnityEngine;
using YKF;

namespace Macchacoffee.ElinMods.NoPCC.UI;

internal class LayerModConfig : YKLayer<object>
{
    public override string Title { get; } = ModConsts.SourceId.ModName;
    public override Rect Bound { get; } = new Rect(0, 0, 440, 440);

    public override void OnLayout()
    {
        CreateTab<LayerModConfigTabMain>(ModConsts.SourceId.ModName, "nopcc.config.main");
    }
}

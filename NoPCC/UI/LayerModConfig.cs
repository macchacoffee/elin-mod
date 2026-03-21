using UnityEngine;
using YKF;

namespace NoPCC.UI;

public class LayerModConfig : YKLayer<object>
{
    public override string Title { get; } = ModNames.NoPCC.Text;
    public override Rect Bound { get; } = new Rect(0, 0, 440, 440);

    public override void OnLayout()
    {
        CreateTab<ModConfigMainTab>(ModNames.NoPCC.Text, "nopcc.config.main");
    }
}
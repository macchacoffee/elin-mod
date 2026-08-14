using UnityEngine;
using YKF;

namespace SimpleDamageTracker.UI.Config;

internal class ModLayerConfig : YKLayer<object>
{
    public override string Title { get; } = $"{ModConsts.SourceId.ModName.lang()}";
    public override Rect Bound { get; } = new Rect(0, 0, 720, 540);

    public override void OnLayout()
    {
        CreateTab<ModLayerConfigTabGenral>(ModConsts.SourceId.ModName, ModConsts.GameObjectName.ConfigGenaral);

        Window.AddBottomButton(ModConsts.SourceId.ResetConfig, () =>
        {
            Dialog.YesNo(ModConsts.SourceId.DialogResetConfig, () =>
            {
                Close();
                ModContext.WorldConfig.ResetDisplay();
                YK.CreateLayer<ModLayerConfig>();
            });
        });
    }
}

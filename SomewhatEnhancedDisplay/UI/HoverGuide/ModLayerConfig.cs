using UnityEngine;
using YKF;

namespace SomewhatEnhancedDisplay.UI.HoverGuide;

public class ModLayerConfig : YKLayer<object>
{
    public override string Title { get; } = $"{ModConsts.SourceId.ModName.lang()} {ModConsts.SourceId.ConfigHoverGuide.lang()}";
    // public override Rect Bound { get; } = new Rect(0, 0, 800, 600);
    public override Rect Bound { get; } = new Rect(0, 0, 720, 540);

    private UIButton? ButtonReset { get; set; }

    public override void OnLayout()
    {
        CreateTab<ModLayerConfigTabGenral>(ModConsts.SourceId.ConfigGeneral, ModConsts.GameObjectName.ConfigGenaral);
        CreateTab<ModLayerConfigTabStyle>(ModConsts.SourceId.ConfigStyle, ModConsts.GameObjectName.ConfigStyle);

        Window.AddBottomButton(ModConsts.SourceId.ResetConfig, () =>
        {
            Dialog.YesNo(ModConsts.SourceId.DialogResetConfig, () =>
            {
                Close();
                Mod.Config.ResetHoverGuide();
                YK.CreateLayer<ModLayerConfig>();
            });
        });

        ButtonReset = Window.AddBottomButton(CurrentTabLang(ModConsts.SourceId.ResetConfigTab), () =>
        {
            Dialog.YesNo(CurrentTabLang(ModConsts.SourceId.DialogResetConfigTab), () =>
            {
                Close();
                if (Window.CurrentTab.idLang == ModConsts.SourceId.ConfigGeneral)
                {
                    Mod.Config.HoverGuide.ResetGeneral();
                }
                else if (Window.CurrentTab.idLang == ModConsts.SourceId.ConfigStyle)
                {
                    Mod.Config.HoverGuide.ResetStyle();
                }
                YK.CreateLayer<ModLayerConfig>();
            });
        });
    }

    public override void OnSwitchContent(Window window)
    {
        ButtonReset?.mainText.SetText(CurrentTabLang(ModConsts.SourceId.ResetConfigTab));
        Window.rectBottom.RebuildLayout(recursive: true);
		Window.CurrentContent.RebuildLayout(recursive: true);
    }

    private string CurrentTabLang(string langId)
    {
        return langId.lang(Window.CurrentTab.idLang.lang());
    }
}
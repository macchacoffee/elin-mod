using SomewhatEnhancedDisplay.Config;
using UnityEngine;
using YKF;

namespace SomewhatEnhancedDisplay.UI.HoverGuide.Config;

public class ModLayerConfig : YKLayer<ModLayerConfigContext>
{
    public override string Title { get; } = $"{ModConsts.SourceId.ModName.lang()} {ModConsts.SourceId.ConfigHoverGuide.lang()}";
    public override Rect Bound { get; } = new Rect(0, 0, 720, 540);

    private UIButton? ButtonReset { get; set; }

    private ModLayerConfigContext Context => Data;
    private static ModConfigHoverGuide Config => Mod.Config.HoverGuide;

    public override string GetTextHeader(Window window)
    {
        var prefix = string.Empty;
        var idLang = Window.CurrentTab.idLang;
        if (idLang == ModConsts.SourceId.ConfigStyleTargetChara
            || idLang == ModConsts.SourceId.ConfigStyleTargetThing)
        {
            prefix = $"[{ModConsts.SourceId.StyleName.lang((Context.SelectedStyleIndex + 1).ToString())}] ";
        }
        return $"{prefix}{ModConsts.SourceId.ConfigOf.lang(base.GetTextHeader(window))}";
    }

    public override void OnLayout()
    {
        // 設定画面での選択中スタイルとホバーガイドの表示スタイルを同期する
        Context.SelectedStyleIndex = Config.CurrentStyleIndex;
        Context.AddSelectedStyleChangedListener(index => Config.CurrentStyleIndex = index);

        CreateTab<ModLayerConfigTabGenral>(ModConsts.SourceId.ConfigGeneral, ModConsts.GameObjectName.ConfigGenaral);
        CreateTab<ModLayerConfigTabStyle>(ModConsts.SourceId.ConfigStyle, ModConsts.GameObjectName.ConfigStyle);
        CreateTab<ModLayerConfigTabStyleTargetChara>(ModConsts.SourceId.ConfigStyleTargetChara, ModConsts.GameObjectName.ConfigStyleTargetChara);
        CreateTab<ModLayerConfigTabStyleTargetThing>(ModConsts.SourceId.ConfigStyleTargetThing, ModConsts.GameObjectName.ConfigStyleTargetThing);

        // タブの画像を設定する
        GetTab(ModConsts.GameObjectName.ConfigStyle).sprite = GetTabIconSprite(85);
        GetTab(ModConsts.GameObjectName.ConfigStyleTargetChara).sprite = GetTabIconSprite(115);
        GetTab(ModConsts.GameObjectName.ConfigStyleTargetThing).sprite = GetTabIconSprite(109);

        ModUI.HoverGuide?.ClearTarget();
        UpdateHoverGuideSample(Context.SampleChara);

        Window.AddBottomButton(ModConsts.SourceId.ResetConfig, () =>
        {
            Dialog.YesNo(ModConsts.SourceId.DialogResetConfig, () =>
            {
                Close();
                Mod.Config.ResetHoverGuide();
                YK.CreateLayer<ModLayerConfig, ModLayerConfigContext>(new(Context.SampleChara, Context.SampleThing));
            });
        });

        ButtonReset = Window.AddBottomButton(CurrentTabLang(ModConsts.SourceId.ResetConfigTab), () =>
        {
            Dialog.YesNo(CurrentTabLang(ModConsts.SourceId.DialogResetConfigTab), () =>
            {
                Close();
                var idLang = Window.CurrentTab.idLang;
                if (idLang == ModConsts.SourceId.ConfigGeneral)
                {
                    Mod.Config.ResetHoverGuideGeneral();    
                }
                else if (
                    idLang == ModConsts.SourceId.ConfigStyle
                    || idLang == ModConsts.SourceId.ConfigStyleTargetChara
                    || idLang == ModConsts.SourceId.ConfigStyleTargetThing)
                {
                    Mod.Config.ResetHoverGuideStyle();
                }
                YK.CreateLayer<ModLayerConfig, ModLayerConfigContext>(new(Context.SampleChara, Context.SampleThing));
            });
        });
    }

    public override void OnKill()
    {
        ModUI.HoverGuide?.UnlockCard();
        ModUI.HoverGuide?.ClearTarget();
    }

    public override void OnSwitchContent(Window window)
    {
        ButtonReset?.mainText.SetText(CurrentTabLang(ModConsts.SourceId.ResetConfigTab));
        Window.rectBottom.RebuildLayout(recursive: true);
        Window.CurrentContent.RebuildLayout(recursive: true);
        UpdateHoverGuideSample();
    }

    private void UpdateHoverGuideSample()
    {
        var idLang = Window.CurrentTab.idLang;
        if (idLang == ModConsts.SourceId.ConfigStyleTargetChara)
        {
            UpdateHoverGuideSample(Context.SampleChara);
        }
        else if (idLang == ModConsts.SourceId.ConfigStyleTargetThing)
        {
            UpdateHoverGuideSample(Context.SampleThing);
        }
    }

    private void UpdateHoverGuideSample(Card card)
    {
        ModUI.HoverGuide?.LockCard(card, Context.SampleModifier);
    }

    private Window.Setting.Tab GetTab(string name)
    {
        return Window.setting.tabs[Window.GetTab(name)];
    }

    private string CurrentTabLang(string langId)
    {
        var idLang = Window.CurrentTab.idLang;
        if (
            idLang == ModConsts.SourceId.ConfigStyleTargetChara
            || idLang == ModConsts.SourceId.ConfigStyleTargetThing)
        {
            // スタイル関連のlangIdはすべてスタイルのlangIdに置き換える
            idLang = ModConsts.SourceId.ConfigStyle;
        }
        return langId.lang(idLang.lang());
    }

    private static Sprite? GetTabIconSprite(int id)
    {
        return SpriteSheet.Get($"{CorePath.Icon}icons_48 static", $"icons_48 static_{id}");
    }
}
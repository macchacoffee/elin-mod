using SomewhatEnhancedDisplay.Config;
using SomewhatEnhancedDisplay.Extensions;
using UnityEngine.UI;
using YKF;

namespace SomewhatEnhancedDisplay.UI.HoverGuide.Config;

public class ModLayerConfigTabStyle : YKLayout<ModLayerConfigContext>
{
    private ModLayerConfigContext Context => Layer.Data;
    private static ModConfigHoverGuide Config => Mod.Config.HoverGuide;

    public override void OnLayout()
    {
        Header(ModConsts.SourceId.ConfigEditStyle);

        Spacer(8);
        var styleLayout = Horizontal().WithFitMode(ContentSizeFitter.FitMode.PreferredSize).WithPivot(0, 0.5f);

        var (_, updateStyleDropdown) = styleLayout.AddModDropdown(
            label: ModConsts.SourceId.SelectStyleToEdit,
            init: 0,
            values: Config.Styles,
            getLabel: (value, index) => ModConsts.SourceId.StyleName.lang((index + 1).ToString()),
            onChanged: (index, Value) => Context.SelectedStyleIndex = index,
            width: 180
        );

        styleLayout.Spacer(0, 40);

        // TODO 追加の選択肢 (全表示、デフォルト表示、表示少なめ) を増やす
        var addStyleButton = styleLayout.AddModButton(
            label: ModConsts.SourceId.AddStyle,
            onClicked: () =>
            {
                Context.AddStyle(new());
            },
            width: 100
        );

        styleLayout.Spacer(0, 6);
        var deleteStyleButton = styleLayout.AddModButton(
            label: ModConsts.SourceId.DeleteStyle,
            onClicked: () =>
            {
                if (Config.Styles.Count <= 1)
                {
                    return;
                }
                var d = Dialog.YesNo(ModConsts.SourceId.DialogDeleteStyle.lang((Context.SelectedStyleIndex + 1).ToString()), () =>
                {
                    Context.DeleteStyle(Context.SelectedStyleIndex);
                });
            },
            width: 80
        );

        void updateStyleButtons()
        {
            updateStyleDropdown(Context.SelectedStyleIndex, Config.Styles);
            addStyleButton.SetInteractableWithAlpha(Config.Styles.Count < 5);
            deleteStyleButton.SetInteractableWithAlpha(Config.Styles.Count > 1);
        };
        Context.AddStyleAddedListener(updateStyleButtons);
        Context.AddStyleDeletedListener(updateStyleButtons);
        updateStyleButtons();

        Spacer(20);
        Header(ModConsts.SourceId.ConfigPreview);

        Spacer(8);
        var previewLayout1 = Horizontal().WithFitMode(ContentSizeFitter.FitMode.PreferredSize).WithPivot(0, 0.5f);

        previewLayout1.AddModButton(
            label: ModConsts.SourceId.PickPreviewChara,
            onClicked: () =>
            {
                Context.UpdateSampleCharaRandom();
                ModUI.HoverGuide?.LockCard(Context.SampleChara, Context.SampleModifier);
            },
            width: 170
        );

        previewLayout1.Spacer(0, 6);
        previewLayout1.AddModButton(
            label: ModConsts.SourceId.PickPreviewThing,
            onClicked: () =>
            {
                Context.UpdateSampleThingRandom();
                ModUI.HoverGuide?.LockCard(Context.SampleThing, Context.SampleModifier);
            },
            width: 240
        );

        Spacer(36);
        var previewLayout2 = Horizontal().WithFitMode(ContentSizeFitter.FitMode.PreferredSize).WithPivot(0, 0.5f);

        previewLayout2.AddModSlider(
            getLabel: value => $"{ModConsts.SourceId.HealthBarRatio.lang()}({value * 100:0}%)",
            init: (float)Context.SampleModifier.HealthBarRatio!,
            min: 0.0f,
            max: 1,
            step: 0.01f,
            onChanged: value => Context.SampleModifier.HealthBarRatio = value
        );
    }
}

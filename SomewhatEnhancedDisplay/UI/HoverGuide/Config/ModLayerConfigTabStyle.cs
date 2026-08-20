using System;
using System.Collections.Generic;

using UnityEngine.UI;
using YKF;

using Macchacoffee.ElinMods.SomewhatEnhancedDisplay.Config;
using Macchacoffee.ElinMods.SomewhatEnhancedDisplay.Extensions;
using UnityEngine;

namespace Macchacoffee.ElinMods.SomewhatEnhancedDisplay.UI.HoverGuide.Config;

internal class ModLayerConfigTabStyle : YKLayout<ModLayerConfigContext>
{
    private const int _minStyleCount = 1;
    private const int _maxStyleCount = 10;

    private Dictionary<string, Func<ModConfigHoverGuideStyle>> StyleFactories { get; }

    private ModLayerConfigContext Context => Layer.Data;
    private static ModConfigHoverGuide Config => ModContext.WorldConfig.HoverGuide;

    public ModLayerConfigTabStyle()
    {
        StyleFactories = new() {
            {ModConsts.SourceId.AddStyleCopy, () => Context.SelectedStyle.DeepCopy()},
            {ModConsts.SourceId.AddStyleMinimal, ModConfigHoverGuideStylePresets.Minimum},
            {ModConsts.SourceId.AddStyleDefault, ModConfigHoverGuideStylePresets.Default},
            {ModConsts.SourceId.AddStyleMaximal, ModConfigHoverGuideStylePresets.Maximal},
        };
    }

    public override void OnLayout()
    {
        Header(ModConsts.SourceId.ConfigEditStyle);

        Spacer(6);
        var styleHeaderLayout = Horizontal().WithFitMode(ContentSizeFitter.FitMode.PreferredSize).WithPivot(0, 0.5f);
        styleHeaderLayout.Text(ModConsts.SourceId.TextConfigEditStyle.lang($"{ModContext.Config.NextStyleKey.Value}"));

        HeaderSmall(ModConsts.SourceId.SelectStyleToEdit);

        var styleLayout1 = Horizontal().WithFitMode(ContentSizeFitter.FitMode.PreferredSize).WithPivot(0, 0.5f);
        var (_, updateStyleDropdown) = styleLayout1.AddModDropdown(
            label: null,
            init: 0,
            values: Config.Styles,
            getLabel: ModLayerConfigContext.GetStyleName,
            onChanged: (index, Value) => Context.SelectedStyleIndex = index,
            width: 240
        );
        styleLayout1.Spacer(0, 6);
        var addStyleButton = styleLayout1.AddModButton(
            label: ModConsts.SourceId.AddStyle,
            onClicked: () =>
            {
                var layer = EClass.ui.AddLayer<LayerList>().SetList2(
                    StyleFactories.Keys,
                    value => value.lang(),
                    (value, _) =>
                    {
                        var style = StyleFactories[value]();
                        Context.AddStyle(style);
                    },
                    null
                 ).SetSize();
                layer.SetHeader(ModConsts.SourceId.SelectStyleTemplate);
            },
            width: 100,
            tooltip: ModConsts.SourceId.TooltipAddStyle.lang($"{_maxStyleCount}")
        );
        var deleteStyleButton = styleLayout1.AddModButton(
            label: ModConsts.SourceId.DeleteStyle,
            onClicked: () =>
            {
                if (Config.Styles.Count <= 1)
                {
                    return;
                }
                var d = Dialog.YesNo(ModConsts.SourceId.DialogDeleteStyle.lang(Context.SelectedStyleName), () =>
                {
                    Context.DeleteStyle(Context.SelectedStyleIndex);
                    SE.Trash();
                });
            },
            width: 80,
            tooltip: ModConsts.SourceId.TooltipDeleteStyle.lang($"{_minStyleCount}")
        );

        HeaderSmall(ModConsts.SourceId.OperationsOnSelectedStyle);

        var styleLayout2 = Horizontal().WithFitMode(ContentSizeFitter.FitMode.PreferredSize).WithPivot(0, 0.5f);
        styleLayout2.AddModButton(
            label: ModConsts.SourceId.RenameStyle,
            onClicked: () =>
            {
                Dialog.InputName(ModConsts.SourceId.DialogRenameStyle, Context.SelectedStyle.Name, (cancel, text) =>
                {
                    if (cancel)
                    {
                        return;
                    }
                    Context.SelectedStyle.Name = text;
                    updateStyleDropdown(Context.SelectedStyleIndex, Config.Styles);
                }, Dialog.InputType.Default);
            },
            width: 180,
            tooltip: ModConsts.SourceId.TooltipRenameStyle
        );

        var styleLayout3 = Horizontal().WithFitMode(ContentSizeFitter.FitMode.PreferredSize).WithPivot(0, 0.5f);
        var moveStyleBackwardButton = styleLayout3.AddModButton(
            label: $"▲ {ModConsts.SourceId.MoveStyleBackward.lang()}",
            onClicked: () => Context.MoveStyleBackward(Context.SelectedStyleIndex),
            width: 150,
            tooltip: ModConsts.SourceId.TooltipMoveStyleBackward
        );
        styleLayout3.Spacer(0, 6);
        var moveStyleforwardButton = styleLayout3.AddModButton(
            label: $"▼ {ModConsts.SourceId.MoveStyleForward.lang()}",
            onClicked: () => Context.MoveStyleForward(Context.SelectedStyleIndex),
            width: 150,
            tooltip: ModConsts.SourceId.TooltipMoveStyleForward
        );

        void updateStyleButtons()
        {
            updateStyleDropdown(Context.SelectedStyleIndex, Config.Styles);
            addStyleButton.SetInteractableWithAlpha(Config.Styles.Count < _maxStyleCount);
            deleteStyleButton.SetInteractableWithAlpha(Config.Styles.Count > _minStyleCount);
            moveStyleBackwardButton.SetInteractableWithAlpha(Config.Styles.Count > 1);
            moveStyleforwardButton.SetInteractableWithAlpha(Config.Styles.Count > 1);
        }
        Context.AddStyleAddedListener((_, _) => updateStyleButtons());
        Context.AddStyleDeletedListener((_, _) => updateStyleButtons());
        Context.AddStyleMovedListener((_, _) => updateStyleButtons());
        updateStyleButtons();

        Spacer(20);
        Header(ModConsts.SourceId.ConfigPreview);

        HeaderSmall(ModConsts.SourceId.PreviewTarget);

        var previewLayout1 = Horizontal().WithFitMode(ContentSizeFitter.FitMode.PreferredSize).WithPivot(0, 0.5f);

        previewLayout1.AddModButton(
            label: ModConsts.SourceId.PickPreviewChara,
            onClicked: () =>
            {
                Context.UpdateSampleCharaRandom();
                ModUI.HoverGuide?.LockCard(Context.SampleChara, Context.SampleModifier);
            },
            width: 170,
            tooltip: ModConsts.SourceId.TooltipPickPreviewChara
        );

        previewLayout1.Spacer(0, 6);
        previewLayout1.AddModButton(
            label: ModConsts.SourceId.PickPreviewThing,
            onClicked: () =>
            {
                Context.UpdateSampleThingRandom();
                ModUI.HoverGuide?.LockCard(Context.SampleThing, Context.SampleModifier);
            },
            width: 240,
            tooltip: ModConsts.SourceId.TooltipPickPreviewThing
        );

        HeaderSmall(ModConsts.SourceId.HealthBar);

        Spacer(36);
        var healthBarPreviewContainer = Vertical();

        var healthRatioPreviewLayout = healthBarPreviewContainer.Vertical();
        healthRatioPreviewLayout.Layout.childAlignment = TextAnchor.UpperLeft;
        var healthRatioPreviewRow = healthRatioPreviewLayout
            .Horizontal()
            .WithFitMode(ContentSizeFitter.FitMode.PreferredSize)
            .WithPivot(0, 0.5f);
        healthRatioPreviewRow.AddModSlider(
            getLabel: value => $"{ModConsts.SourceId.HealthRatio.lang()}({value}%)",
            init: (float)Context.SampleModifier.HealthBarRatio! * 100,
            min: 0,
            max: 100,
            step: 1,
            onChanged: value => Context.SampleModifier.HealthBarRatio = value / 100
        );

        var manaBodyPreviewLayout = healthBarPreviewContainer.Vertical();
        manaBodyPreviewLayout.Layout.childAlignment = TextAnchor.UpperLeft;
        var hpPreviewRow = manaBodyPreviewLayout
            .Horizontal()
            .WithFitMode(ContentSizeFitter.FitMode.PreferredSize)
            .WithPivot(0, 0.5f);
        hpPreviewRow.AddModSlider(
            getLabel: value => $"HP({value}%)",
            init: (float)Context.SampleModifier.HealthBarHPRatio! * 100,
            min: 0,
            max: 100,
            step: 1,
            onChanged: value => Context.SampleModifier.HealthBarHPRatio = value / 100
        );
        manaBodyPreviewLayout.Spacer(36);
        var mpPreviewRow = manaBodyPreviewLayout
            .Horizontal()
            .WithFitMode(ContentSizeFitter.FitMode.PreferredSize)
            .WithPivot(0, 0.5f);
        mpPreviewRow.AddModSlider(
            getLabel: value => $"MP({value}%)",
            init: (float)Context.SampleModifier.HealthBarMPRatio! * 100,
            min: 0,
            max: 100,
            step: 1,
            onChanged: value => Context.SampleModifier.HealthBarMPRatio = value / 100
        );

        void updateHealthBarPreviewLayout()
        {
            var useManaBodyPreview = Context.SelectedStyle.Chara.HealthBar.SplitManaBodyHealthBar
                && Context.SampleChara.Evalue(FEAT.featManaMeat) > 0;
            healthRatioPreviewLayout.Layout.gameObject.SetActive(!useManaBodyPreview);
            manaBodyPreviewLayout.Layout.gameObject.SetActive(useManaBodyPreview);
            healthBarPreviewContainer.RebuildLayout(recursive: true);
        }
        Context.AddSelectedStyleChangedListener((_, _) => updateHealthBarPreviewLayout());
        Context.AddHealthBarPreviewChangedListener(updateHealthBarPreviewLayout);
        updateHealthBarPreviewLayout();
    }
}

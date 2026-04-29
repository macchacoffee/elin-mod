using System;
using SomewhatEnhancedDisplay.Config;
using SomewhatEnhancedDisplay.Extensions;
using UnityEngine;
using UnityEngine.UI;
using YKF;

namespace SomewhatEnhancedDisplay.UI.HoverGuide;

public class ModLayerConfigTabGenral : YKLayout<object>
{
    private static ModConfigHoverGuide Config => Mod.Config.HoverGuide;
    private static ModConfigHoverGuideColorSet ColorConfig => Config.ColorSet;

    public override void OnLayout()
    {
        Header(ModConsts.SourceId.ConfigDisplay);
        Spacer(36);

        var displayLayout1 = Horizontal().WithFitMode(ContentSizeFitter.FitMode.PreferredSize).WithPivot(0, 0.5f);
        displayLayout1.AddModSlider(
            getLabel: value => $"{ModConsts.SourceId.ZoomScale.lang()}({value * 100}%)",
            init: Config.ZoomScale,
            min: 0.5f,
            max: 2,
            step: 0.05f,
            onChanged: value => Config.ZoomScale = value
        );

        Spacer(36);

        var displayLayout2 = Horizontal().WithFitMode(ContentSizeFitter.FitMode.PreferredSize).WithPivot(0, 0.5f);
        displayLayout2.AddModSlider(
            getLabel: value => $"{ModConsts.SourceId.HorizontalPivot.lang()}({value})",
            init: Config.HorizontalPivot,
            min: 0,
            max: 1,
            step: 0.1f,
            onChanged: value => Config.HorizontalPivot = value
        );

        displayLayout2.Spacer(0, 18);
        displayLayout2.AddModSlider(
            getLabel: value => $"{ModConsts.SourceId.VerticalPivot.lang()}({value})",
            init: Config.VerticalPivot,
            min: 0,
            max: 1,
            step: 0.1f,
            onChanged: value => Config.VerticalPivot = value
        );

        var cellWidth = 200;
        var maxColumn = 3;

        Spacer(20);
        Header(ModConsts.SourceId.ConfigColors);

        AddColorPickers(
            layout: this,
            headerLabel: ModConsts.SourceId.DefaultColor,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.MainTextColor,
                Init: ColorConfig.MainTextColor,
                OnChanged: color => ColorConfig.MainTextColor = color
            ),
            new(
                Label: ModConsts.SourceId.SubTextColor,
                Init: ColorConfig.SubTextColor,
                OnChanged: color => ColorConfig.SubTextColor = color
            )
        );

        AddColorPickers(
            layout: this,
            headerLabel: ModConsts.SourceId.HP,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.AttributeLabelColor,
                Init: ColorConfig.HPLabelColor,
                OnChanged: color => ColorConfig.HPLabelColor = color
            ),
            new(
                Label: ModConsts.SourceId.AttributeValueColor,
                Init: ColorConfig.HPValueColor,
                OnChanged: color => ColorConfig.HPValueColor = color
            )
        );

        AddColorPickers(
            layout: this,
            headerLabel: ModConsts.SourceId.Mana,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.AttributeLabelColor,
                Init: ColorConfig.ManaLabelColor,
                OnChanged: color => ColorConfig.ManaLabelColor = color
            ),
            new(
                Label: ModConsts.SourceId.AttributeValueColor,
                Init: ColorConfig.ManaValueColor,
                OnChanged: color => ColorConfig.ManaValueColor = color
            )
        );

        AddColorPickers(
            layout: this,
            headerLabel: ModConsts.SourceId.Stamina,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.AttributeLabelColor,
                Init: ColorConfig.StaminaLabelColor,
                OnChanged: color => ColorConfig.StaminaLabelColor = color
            ),
            new(
                Label: ModConsts.SourceId.AttributeValueColor,
                Init: ColorConfig.StaminaValueColor,
                OnChanged: color => ColorConfig.StaminaValueColor = color
            )
        );

        AddColorPickers(
            layout: this,
            headerLabel: ModConsts.SourceId.Resist,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.ResistLabelColor,
                Init: ColorConfig.ResistLabelColor,
                OnChanged: color => ColorConfig.ResistLabelColor = color
            ),
            new(
                Label: ModConsts.SourceId.NegativeResistLabelColor,
                Init: ColorConfig.NegativeResistLabelColor,
                OnChanged: color => ColorConfig.NegativeResistLabelColor = color
            ),
            new(
                Label: ModConsts.SourceId.NoneResistLabelColor,
                Init: ColorConfig.NoneResistLabelColor,
                OnChanged: color => ColorConfig.NoneResistLabelColor = color
            )
        );

        AddColorPickers(
            layout: this,
            headerLabel: ModConsts.SourceId.HealthBar,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.HealthBarBGColor,
                Init: ColorConfig.HealthBarBGColor,
                OnChanged: color => ColorConfig.HealthBarBGColor = color
            ),
            new(
                Label: ModConsts.SourceId.HealthBarFGColor,
                Init: ColorConfig.HealthBarFGColor,
                OnChanged: color => ColorConfig.HealthBarFGColor = color
            ),
            new(
                Label: ModConsts.SourceId.HealthBarFGRestoreColor,
                Init: ColorConfig.HealthBarFGRestoreColor,
                OnChanged: color => ColorConfig.HealthBarFGRestoreColor = color
            ),
            new(
                Label: ModConsts.SourceId.HealthBarFGDamageColor,
                Init: ColorConfig.HealthBarFGDamageColor,
                OnChanged: color => ColorConfig.HealthBarFGDamageColor = color
            ),
            new(
                Label: ModConsts.SourceId.HealthBarLowValueFGColor,
                Init: ColorConfig.HealthBarLowValueFGColor,
                OnChanged: color => ColorConfig.HealthBarLowValueFGColor = color
            ),
            new(
                Label: ModConsts.SourceId.HealthBarTextColor,
                Init: ColorConfig.HealthBarTextColor,
                OnChanged: color => ColorConfig.HealthBarTextColor = color
            ),
            new(
                Label: ModConsts.SourceId.HealthBarLowValueTextColor,
                Init: ColorConfig.HealthBarLowValueTextColor,
                OnChanged: color => ColorConfig.HealthBarLowValueTextColor = color
            )
        );

        AddColorPickers(
            layout: this,
            headerLabel: ModConsts.SourceId.Rarity,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.RarityCrudeColor,
                Init: ColorConfig.RarityCrudeColor,
                OnChanged: color => ColorConfig.RarityCrudeColor = color
            ),
            new(
                Label: ModConsts.SourceId.RarityNormalColor,
                Init: ColorConfig.RarityNormalColor,
                OnChanged: color => ColorConfig.RarityNormalColor = color
            ),
            new(
                Label: ModConsts.SourceId.RaritySuperiorColor,
                Init: ColorConfig.RaritySuperiorColor,
                OnChanged: color => ColorConfig.RaritySuperiorColor = color
            ),
            new(
                Label: ModConsts.SourceId.RarityLegendaryColor,
                Init: ColorConfig.RarityLegendaryColor,
                OnChanged: color => ColorConfig.RarityLegendaryColor = color
            ),
            new(
                Label: ModConsts.SourceId.RarityMythicalColor,
                Init: ColorConfig.RarityMythicalColor,
                OnChanged: color => ColorConfig.RarityMythicalColor = color
            ),
            new(
                Label: ModConsts.SourceId.RarityArtifactColor,
                Init: ColorConfig.RarityArtifactColor,
                OnChanged: color => ColorConfig.RarityArtifactColor = color
            )
        );

        AddColorPickers(
            layout: this,
            headerLabel: ModConsts.SourceId.Fressness,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.FressnessValueColor,
                Init: ColorConfig.FressnessValueColor,
                OnChanged: color => ColorConfig.FressnessValueColor = color
            ),
            new(
                Label: ModConsts.SourceId.FressnessLowValueColor,
                Init: ColorConfig.FressnessLowValueColor,
                OnChanged: color => ColorConfig.FressnessLowValueColor = color
            )
        );
    }

    private record ColorConfigItem(string Label, Color? Init, Action<Color> OnChanged);

    private void AddColorPickers(YKLayout layout, string headerLabel, int cellWidth, int maxColumn, params ColorConfigItem[] items)
    {
        layout.HeaderSmall(headerLabel);
        var grid = layout.Grid().WithPivot(0, 0.5f).WithCellSize(cellWidth, 50).WithConstraintCount(maxColumn);
        grid.Layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        foreach (var item in items)
        {
            grid.AddModColorPicker(item.Label, item.Init, item.OnChanged);
        }
    }
}

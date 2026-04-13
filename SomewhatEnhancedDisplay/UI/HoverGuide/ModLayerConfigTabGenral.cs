using System;
using System.Linq;
using SomewhatEnhancedDisplay.Config;
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

        var displayLayout = Horizontal().WithFitMode(ContentSizeFitter.FitMode.PreferredSize).WithPivot(0, 0.5f);
        AddSlider(
            layout: displayLayout,
            label: ModConsts.SourceId.ZoomScale,
            init: Config.ZoomScale,
            min: 0.1f,
            max: 2,
            step: 0.1f,
            valueChangedFunc: value => Config.ZoomScale = value
        );

        var cellWidth = 200;
        var maxColumn = 3;

        Spacer(20);
        Header(ModConsts.SourceId.ConfigColors);

        AddColorSettings(
            layout: this,
            headerLabel: ModConsts.SourceId.DefaultColor,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.MainTextColor,
                InitColor: ColorConfig.MainTextColor,
                ColorChangedFunc: color => ColorConfig.MainTextColor = color
            ),
            new(
                Label: ModConsts.SourceId.SubTextColor,
                InitColor: ColorConfig.SubTextColor,
                ColorChangedFunc: color => ColorConfig.SubTextColor = color
            )
        );

        AddColorSettings(
            layout: this,
            headerLabel: ModConsts.SourceId.HP,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.AttributeLabelColor,
                InitColor: ColorConfig.HPLabelColor,
                ColorChangedFunc: color => ColorConfig.HPLabelColor = color
            ),
            new(
                Label: ModConsts.SourceId.AttributeValueColor,
                InitColor: ColorConfig.HPValueColor,
                ColorChangedFunc: color => ColorConfig.HPValueColor = color
            )
        );

        AddColorSettings(
            layout: this,
            headerLabel: ModConsts.SourceId.Mana,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.AttributeLabelColor,
                InitColor: ColorConfig.ManaLabelColor,
                ColorChangedFunc: color => ColorConfig.ManaLabelColor = color
            ),
            new(
                Label: ModConsts.SourceId.AttributeValueColor,
                InitColor: ColorConfig.ManaValueColor,
                ColorChangedFunc: color => ColorConfig.ManaValueColor = color
            )
        );

        AddColorSettings(
            layout: this,
            headerLabel: ModConsts.SourceId.Stamina,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.AttributeLabelColor,
                InitColor: ColorConfig.StaminaLabelColor,
                ColorChangedFunc: color => ColorConfig.StaminaLabelColor = color
            ),
            new(
                Label: ModConsts.SourceId.AttributeValueColor,
                InitColor: ColorConfig.StaminaValueColor,
                ColorChangedFunc: color => ColorConfig.StaminaValueColor = color
            )
        );

        AddColorSettings(
            layout: this,
            headerLabel: ModConsts.SourceId.Resist,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.ResistLabelColor,
                InitColor: ColorConfig.ResistLabelColor,
                ColorChangedFunc: color => ColorConfig.ResistLabelColor = color
            ),
            new(
                Label: ModConsts.SourceId.NegativeResistLabelColor,
                InitColor: ColorConfig.NegativeResistLabelColor,
                ColorChangedFunc: color => ColorConfig.NegativeResistLabelColor = color
            ),
            new(
                Label: ModConsts.SourceId.NoneResistLabelColor,
                InitColor: ColorConfig.NoneResistLabelColor,
                ColorChangedFunc: color => ColorConfig.NoneResistLabelColor = color
            )
        );

        AddColorSettings(
            layout: this,
            headerLabel: ModConsts.SourceId.HealthBar,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.HealthBarBGColor,
                InitColor: ColorConfig.HealthBarBGColor,
                ColorChangedFunc: color => ColorConfig.HealthBarBGColor = color
            ),
            new(
                Label: ModConsts.SourceId.HealthBarFGColor,
                InitColor: ColorConfig.HealthBarFGColor,
                ColorChangedFunc: color => ColorConfig.HealthBarFGColor = color
            ),
            new(
                Label: ModConsts.SourceId.HealthBarFGDamageColor,
                InitColor: ColorConfig.HealthBarFGDamageColor,
                ColorChangedFunc: color => ColorConfig.HealthBarFGDamageColor = color
            ),
            new(
                Label: ModConsts.SourceId.HealthBarLowValueFGColor,
                InitColor: ColorConfig.HealthBarLowValueFGColor,
                ColorChangedFunc: color => ColorConfig.HealthBarLowValueFGColor = color
            ),
            new(
                Label: ModConsts.SourceId.HealthBarTextColor,
                InitColor: ColorConfig.HealthBarTextColor,
                ColorChangedFunc: color => ColorConfig.HealthBarTextColor = color
            ),
            new(
                Label: ModConsts.SourceId.HealthBarLowValueTextColor,
                InitColor: ColorConfig.HealthBarLowValueTextColor,
                ColorChangedFunc: color => ColorConfig.HealthBarLowValueTextColor = color
            )
        );

        AddColorSettings(
            layout: this,
            headerLabel: ModConsts.SourceId.Rarity,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.RarityCrudeColor,
                InitColor: ColorConfig.RarityCrudeColor,
                ColorChangedFunc: color => ColorConfig.RarityCrudeColor = color
            ),
            new(
                Label: ModConsts.SourceId.RarityNormalColor,
                InitColor: ColorConfig.RarityNormalColor,
                ColorChangedFunc: color => ColorConfig.RarityNormalColor = color
            ),
            new(
                Label: ModConsts.SourceId.RaritySuperiorColor,
                InitColor: ColorConfig.RaritySuperiorColor,
                ColorChangedFunc: color => ColorConfig.RaritySuperiorColor = color
            ),
            new(
                Label: ModConsts.SourceId.RarityLegendaryColor,
                InitColor: ColorConfig.RarityLegendaryColor,
                ColorChangedFunc: color => ColorConfig.RarityLegendaryColor = color
            ),
            new(
                Label: ModConsts.SourceId.RarityMythicalColor,
                InitColor: ColorConfig.RarityMythicalColor,
                ColorChangedFunc: color => ColorConfig.RarityMythicalColor = color
            ),
            new(
                Label: ModConsts.SourceId.RarityArtifactColor,
                InitColor: ColorConfig.RarityArtifactColor,
                ColorChangedFunc: color => ColorConfig.RarityArtifactColor = color
            )
        );

        AddColorSettings(
            layout: this,
            headerLabel: ModConsts.SourceId.Fressness,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.FressnessValueColor,
                InitColor: ColorConfig.FressnessValueColor,
                ColorChangedFunc: color => ColorConfig.FressnessValueColor = color
            ),
            new(
                Label: ModConsts.SourceId.FressnessLowValueColor,
                InitColor: ColorConfig.FressnessLowValueColor,
                ColorChangedFunc: color => ColorConfig.FressnessLowValueColor = color
            )
        );
    }

    private record ColorSettingItem(string Label, Color? InitColor, Action<Color> ColorChangedFunc);

    private void AddColorSettings(YKLayout layout, string headerLabel, int cellWidth, int maxColumn, params ColorSettingItem[] items)
    {
        layout.HeaderSmall(headerLabel);
        var grid = layout.Grid().WithPivot(0, 0.5f).WithCellSize(cellWidth, 50).WithConstraintCount(maxColumn);
        grid.Layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        foreach (var item in items)
        {
            AddColorPicker(grid, item.Label, item.InitColor, item.ColorChangedFunc);
        }
    }

    private Slider AddSlider(YKLayout layout, string label, float init, float min, float max, float step, Action<float> valueChangedFunc, int? width = null)
    {
        if (step <= 0)
        {
            new ArgumentException("Step of slider must be more than 0");
        }

        var slider = layout.Slider(
            init / step,
            value => valueChangedFunc(value * step),
            min / step,
            max / step,
            value => $"{label.lang()}(x{value * step})"
        ).WithWidth(width ?? 200);
        slider.wholeNumbers = true;

        return slider;
    }

    private ButtonGeneral AddColorPicker(YKLayout layout, string label, Color? init, Action<Color> colorChangedFunc)
    {
        var initColor = init ?? Color.clear;

        var button = Util.Instantiate<ButtonGeneral>($"{CorePath.UI.Button}ButtonColor", layout);
        button.icon.color = initColor;
        button.mainText.text = label.lang();
        button.LayoutElement().preferredWidth = button.mainText.preferredWidth + button.image.preferredWidth;
        button.SetOnClick(() =>
        {
            var colorPicker = EClass.ui.AddLayer<LayerColorPicker>();
            colorPicker.SetColor(button.icon.color , button.icon.color, (state, color) =>
            {
                switch (state)
                {
                    case PickerState.Confirm:
                        button.icon.color = color;
                        break;
                    case PickerState.Reset:
                    case PickerState.Cancel:
                        color = initColor;
                        break;
                }
                colorChangedFunc(color);
            });
        });

        return button;
    }
}

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

    public override void OnLayout()
    {
        Header(ModConsts.SourceId.ConfigDisplay);
        Spacer(36);
        var displayLayout = Horizontal().WithFitMode(ContentSizeFitter.FitMode.PreferredSize).WithPivot(0, 0.5f);
        AddSlider(
            displayLayout,
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
                InitColor: Config.MainTextColor,
                ColorChangedFunc: color => Config.MainTextColor = color
            ),
            new(
                Label: ModConsts.SourceId.SubTextColor,
                InitColor: Config.SubTextColor,
                ColorChangedFunc: color => Config.SubTextColor = color
            )
        );

        AddColorSettings(
            layout: this,
            headerLabel: ModConsts.SourceId.HP,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.AttributeLabelColor,
                InitColor: Config.HPLabelColor,
                ColorChangedFunc: color => Config.HPLabelColor = color
            ),
            new(
                Label: ModConsts.SourceId.AttributeValueColor,
                InitColor: Config.HPValueColor,
                ColorChangedFunc: color => Config.HPValueColor = color
            )
        );

        AddColorSettings(
            layout: this,
            headerLabel: ModConsts.SourceId.Mana,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.AttributeLabelColor,
                InitColor: Config.ManaLabelColor,
                ColorChangedFunc: color => Config.ManaLabelColor = color
            ),
            new(
                Label: ModConsts.SourceId.AttributeValueColor,
                InitColor: Config.ManaValueColor,
                ColorChangedFunc: color => Config.ManaValueColor = color
            )
        );

        AddColorSettings(
            layout: this,
            headerLabel: ModConsts.SourceId.Stamina,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.AttributeLabelColor,
                InitColor: Config.StaminaLabelColor,
                ColorChangedFunc: color => Config.StaminaLabelColor = color
            ),
            new(
                Label: ModConsts.SourceId.AttributeValueColor,
                InitColor: Config.StaminaValueColor,
                ColorChangedFunc: color => Config.StaminaValueColor = color
            )
        );

        AddColorSettings(
            layout: this,
            headerLabel: ModConsts.SourceId.Resist,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.ResistLabelColor,
                InitColor: Config.ResistLabelColor,
                ColorChangedFunc: color => Config.ResistLabelColor = color
            ),
            new(
                Label: ModConsts.SourceId.NegativeResistLabelColor,
                InitColor: Config.NegativeResistLabelColor,
                ColorChangedFunc: color => Config.NegativeResistLabelColor = color
            ),
            new(
                Label: ModConsts.SourceId.NoneResistLabelColor,
                InitColor: Config.NoneResistLabelColor,
                ColorChangedFunc: color => Config.NoneResistLabelColor = color
            )
        );

        AddColorSettings(
            layout: this,
            headerLabel: ModConsts.SourceId.HealthBar,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.HealthBarBGColor,
                InitColor: Config.HealthBarBGColor,
                ColorChangedFunc: color => Config.HealthBarBGColor = color
            ),
            new(
                Label: ModConsts.SourceId.HealthBarFGColor,
                InitColor: Config.HealthBarFGColor,
                ColorChangedFunc: color => Config.HealthBarFGColor = color
            ),
            new(
                Label: ModConsts.SourceId.HealthBarFGDamageColor,
                InitColor: Config.HealthBarFGDamageColor,
                ColorChangedFunc: color => Config.HealthBarFGDamageColor = color
            ),
            new(
                Label: ModConsts.SourceId.HealthBarLowValueFGColor,
                InitColor: Config.HealthBarLowValueFGColor,
                ColorChangedFunc: color => Config.HealthBarLowValueFGColor = color
            ),
            new(
                Label: ModConsts.SourceId.HealthBarTextColor,
                InitColor: Config.HealthBarTextColor,
                ColorChangedFunc: color => Config.HealthBarTextColor = color
            ),
            new(
                Label: ModConsts.SourceId.HealthBarLowValueTextColor,
                InitColor: Config.HealthBarLowValueTextColor,
                ColorChangedFunc: color => Config.HealthBarLowValueTextColor = color
            )
        );

        AddColorSettings(
            layout: this,
            headerLabel: ModConsts.SourceId.Rarity,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.RarityCrudeColor,
                InitColor: Config.RarityCrudeColor,
                ColorChangedFunc: color => Config.RarityCrudeColor = color
            ),
            new(
                Label: ModConsts.SourceId.RarityNormalColor,
                InitColor: Config.RarityNormalColor,
                ColorChangedFunc: color => Config.RarityNormalColor = color
            ),
            new(
                Label: ModConsts.SourceId.RaritySuperiorColor,
                InitColor: Config.RaritySuperiorColor,
                ColorChangedFunc: color => Config.RaritySuperiorColor = color
            ),
            new(
                Label: ModConsts.SourceId.RarityLegendaryColor,
                InitColor: Config.RarityLegendaryColor,
                ColorChangedFunc: color => Config.RarityLegendaryColor = color
            ),
            new(
                Label: ModConsts.SourceId.RarityMythicalColor,
                InitColor: Config.RarityMythicalColor,
                ColorChangedFunc: color => Config.RarityMythicalColor = color
            ),
            new(
                Label: ModConsts.SourceId.RarityArtifactColor,
                InitColor: Config.RarityArtifactColor,
                ColorChangedFunc: color => Config.RarityArtifactColor = color
            )
        );

        AddColorSettings(
            layout: this,
            headerLabel: ModConsts.SourceId.Fressness,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.FressnessValueColor,
                InitColor: Config.FressnessValueColor,
                ColorChangedFunc: color => Config.FressnessValueColor = color
            ),
            new(
                Label: ModConsts.SourceId.FressnessLowValueColor,
                InitColor: Config.FressnessLowValueColor,
                ColorChangedFunc: color => Config.FressnessLowValueColor = color
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

    private void AddSlider(YKLayout layout, string label, float init, float min, float max, float step, Action<float> valueChangedFunc, int? width = null)
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
    }

    private void AddColorPicker(YKLayout layout, string label, Color? init, Action<Color> colorChangedFunc)
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
    }
}

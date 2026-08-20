using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using YKF;

using Macchacoffee.ElinMods.SimpleDamageTracker.Config;
using Macchacoffee.ElinMods.SimpleDamageTracker.Extensions;

namespace Macchacoffee.ElinMods.SimpleDamageTracker.UI.Config;

internal class ModLayerConfigTabGenral : YKLayout<object>
{
    private const int _cellWidth1 = 200;
    private const int _maxColumn1 = 3;

    private static readonly Dictionary<ModHorizontalTextAlignment, string> _itemHorizontalTextAlignmentIdLangs = new() {
        {ModHorizontalTextAlignment.Left, ModConsts.SourceId.AlignmentLeft},
        {ModHorizontalTextAlignment.Center, ModConsts.SourceId.AlignmentCenter},
        {ModHorizontalTextAlignment.Right, ModConsts.SourceId.AlignmentRight}
    };
    private static readonly List<ModHorizontalTextAlignment> _itemHorizontalTextAlignments = [.. _itemHorizontalTextAlignmentIdLangs.Keys];

    private static ModConfigDisplay Config => ModContext.WorldConfig.Display;

    private UIManager _ui = new();
 
    public override void OnLayout()
    {

        Header(ModConsts.SourceId.ConfigGeneral);

        _ui.Add(
            layout: this,
            headerLabel: null,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
            new ToogleUIItem(
                Label: ModConsts.SourceId.DisplayNoDamage,
                Init: Config.DisplayNoDamage,
                OnChanged: value => Config.DisplayNoDamage = value
            ),
            new ToogleUIItem(
                Label: ModConsts.SourceId.UseAnimation,
                Init: Config.UseAnimation,
                OnChanged: value => Config.UseAnimation = value
            ),
            new ToogleUIItem(
                Label: ModConsts.SourceId.UseCompactDamageFormat,
                Init: Config.UseCompactDamageFormat,
                OnChanged: value => Config.UseCompactDamageFormat = value,
                Tooltip:  ModConsts.SourceId.TooltipUseCompactDamageFormat
            )
        );

        Header(ModConsts.SourceId.ConfigDisplayItems);

        AddDisplayTextUI(Config.Damage, ModConsts.SourceId.ConfigDamage);
        AddDisplayTextUI(Config.DamageShare, ModConsts.SourceId.ConfigDamageShare);
    }

    private void AddDisplayTextUI(ModConfigDisplayText config, string headerLabel)
    {
        _ui.Add(
            layout: this,
            headerLabel: headerLabel,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
            new ToogleUIItem(
                Label: ModConsts.SourceId.Display,
                Init: config.Display,
                OnChanged: value => config.Display = value
            ),
            new ColorUIItem(
                Label: ModConsts.SourceId.Color,
                Init: config.Color,
                OnChanged: value => config.Color = value
            ),
            CreateHorizontalTextAlignmentDropdownUIItem(
                label: ModConsts.SourceId.HorizontalAlignment,
                init: config.HorizontalAlignment,
                onChanged: value => config.HorizontalAlignment = value
            ),
            new SliderUIItem(
                GetLabel: value => $"{ModConsts.SourceId.PositionX.lang()}({value})",
                Init: config.X,
                Min: -100,
                Max: 100,
                Step: 1,
                OnChanged: value => config.X = value
            ),
            new SliderUIItem(
                GetLabel: value => $"{ModConsts.SourceId.PositionY.lang()}({value})",
                Init: config.Y,
                Min: -100,
                Max: 100,
                Step: 1,
                OnChanged: value => config.Y = value
            ),
            new SliderUIItem(
                GetLabel: value => $"{ModConsts.SourceId.SizeScale.lang()}({value * 100}%)",
                Init: config.SizeScale,
                Min: 0.5f,
                Max: 2,
                Step: 0.05f,
                OnChanged: value => config.SizeScale = value
            )
        );
    }

    private interface IUIItem
    {
        public void AddUI(YKLayout layout);
    };

    private record ToogleUIItem(
        string Label,
        bool Init,
        Action<bool> OnChanged,
        string? Tooltip = null) : IUIItem
    {
        public void AddUI(YKLayout layout)
        {
            layout.AddModToggle(Label, Init, OnChanged, Tooltip);;
        }
    }

    private record SliderUIItem(
        Func<float, string> GetLabel,
        float Init,
        float Min,
        float Max,
        float Step,
        Action<float> OnChanged) : IUIItem
    {
        public void AddUI(YKLayout layout)
        {
            var layout2 = layout.Horizontal();
            layout2.Layout.childAlignment = TextAnchor.LowerLeft;
            layout2.Fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            layout2.Spacer(0, 20);
            layout2.AddModSlider(GetLabel, Init, Min, Max, Step, OnChanged);
        }
    }

    private record DropdownUIItem<T>(
         string? Label,
         int Init,
         IEnumerable<T> Values,
         Func<int, T, string> GetLabel,
         Action<int, T> OnChanged) : IUIItem
    {
        public void AddUI(YKLayout layout)
        {
            layout.AddModDropdown(Label, Init, Values, GetLabel, OnChanged);
        }
    }

    private DropdownUIItem<ModHorizontalTextAlignment> CreateHorizontalTextAlignmentDropdownUIItem(string? label, ModHorizontalTextAlignment init, Action<ModHorizontalTextAlignment> onChanged)
    {
        return new(
            Label: label,
            Init: _itemHorizontalTextAlignments.IndexOf(init),
            Values: _itemHorizontalTextAlignments,
            GetLabel: (_, value) => _itemHorizontalTextAlignmentIdLangs[value].lang(),
            OnChanged: (_, value) => onChanged(value)
        );
    }

    private record ColorUIItem(
         string Label,
         Color Init,
        Action<Color> OnChanged,
        string? Tooltip = null) : IUIItem
    {
        public void AddUI(YKLayout layout)
        {
            layout.AddModColorPicker(Label, Init, OnChanged, Tooltip);
        }
    }

    private class UIManager
    {
        public void Add(YKLayout layout, string? headerLabel, int cellWidth, int maxColumn, params IUIItem[] items)
        {
            if (headerLabel is not null)
            {
                layout.HeaderSmall(headerLabel);
            }
            var grid = layout.Grid().WithPivot(0, 0.5f).WithCellSize(cellWidth, 50).WithConstraintCount(maxColumn);
            grid.Layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            foreach (var item in items)
            {
                item.AddUI(grid);
            }
        }
    }
}

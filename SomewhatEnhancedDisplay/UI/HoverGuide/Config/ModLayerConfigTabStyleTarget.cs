using System;
using System.Collections.Generic;
using SomewhatEnhancedDisplay.Config;
using SomewhatEnhancedDisplay.Extensions;
using UnityEngine.UI;
using YKF;

namespace SomewhatEnhancedDisplay.UI.HoverGuide.Config;

public abstract class ModLayerConfigTabStyleTarget : YKLayout<ModLayerConfigContext>
{
    protected EditStyleUIManager EditStyleUI { get; private set; } = new();

    protected ModLayerConfigContext Context => Layer.Data;
    protected ModConfigHoverGuideStyle SelectedStyle => Context.SelectedStyle;

    public override void OnLayout()
    {
        EditStyleUI = new();
        OnLayoutInternal();
        Context.AddSelectedStyleChangedListener(_ => EditStyleUI.OnStyleChanged());
    }

    protected abstract void OnLayoutInternal();

    protected interface IEditStyleUIItem
    {
        public UIListenerSet AddUI(YKLayout layout);
    };

    protected record EditStyleToogleUIItem(
        string Label,
        bool Init,
        Action<bool> OnChanged,
        Func<bool> GetConfig,
        string? Tooltip = null) : IEditStyleUIItem
    {
        public UIListenerSet AddUI(YKLayout layout)
        {
            var toogle = layout.AddModToggle(Label, Init, OnChanged);
            if (Tooltip is string tooltip)
            {
                toogle.SetTooltipLang(tooltip);
                toogle.tooltip.icon = true;
            }
            return new(OnSelectedStyleChanged: () => toogle.SetCheck(GetConfig()));
        }
    }

    protected record EditStyleSliderUIItem(
        Func<float, string> GetLabel,
        float Init,
        float Min,
        float Max,
        float Step,
        Action<float> OnChanged,
        Func<float> GetConfig) : IEditStyleUIItem
    {
        public UIListenerSet AddUI(YKLayout layout)
        {
            var layout2 = layout.Horizontal();
            layout2.Layout.childAlignment = UnityEngine.TextAnchor.LowerLeft;
            layout2.Fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            layout2.Spacer(0, 20);
            var slider = layout2.AddModSlider(GetLabel, Init, Min, Max, Step, OnChanged);
            return new(OnSelectedStyleChanged: () => slider.value = GetConfig());
        }
    }

    protected record UIListenerSet(Action? OnSelectedStyleChanged = null);

    protected class EditStyleUIManager
    {
        private List<Action> SelectedStyleChangedListeners { get; } = [];

        public void OnStyleChanged()
        {
            foreach (var listener in SelectedStyleChangedListeners)
            {
                listener();
            }
        }

        public void Add(YKLayout layout, string? headerLabel, int cellWidth, int maxColumn, params IEditStyleUIItem[] items)
        {
            if (headerLabel is not null)
            {
                layout.HeaderSmall(headerLabel);
            }
            var grid = layout.Grid().WithPivot(0, 0.5f).WithCellSize(cellWidth, 50).WithConstraintCount(maxColumn);
            grid.Layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            foreach (var item in items)
            {
                var listenerSet = item.AddUI(grid);
                if (listenerSet.OnSelectedStyleChanged is not null)
                {
                     SelectedStyleChangedListeners.Add(listenerSet.OnSelectedStyleChanged);
                }
            }
        }
    }
}

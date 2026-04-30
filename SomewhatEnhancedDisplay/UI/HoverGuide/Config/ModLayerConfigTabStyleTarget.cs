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

    protected record EditStyleToogleUIItem(string Label, bool Init, Action<bool> OnChanged, Func<bool> GetConfig, string? Tooltip = null);

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

        public void AddToggle(YKLayout layout, string headerLabel, int cellWidth, int maxColumn, EditStyleToogleUIItem item)
        {
            AddToggles(layout, headerLabel, cellWidth, maxColumn, [item]);
        }

        public void AddToggles(YKLayout layout, string? headerLabel, int cellWidth, int maxColumn, params EditStyleToogleUIItem[] items)
        {
            if (headerLabel is not null)
            {
                var header = layout.HeaderSmall(headerLabel);
            }
            var grid = layout.Grid().WithPivot(0, 0.5f).WithCellSize(cellWidth, 50).WithConstraintCount(maxColumn);
            grid.Layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            foreach (var item in items)
            {
                var toogle = grid.AddModToggle(item.Label, item.Init, item.OnChanged);
                if (item.Tooltip is string tooltip)
                {
                    toogle.SetTooltipLang(tooltip);
                    toogle.tooltip.icon = true;
                }
                SelectedStyleChangedListeners.Add(() => toogle.SetCheck(item.GetConfig()));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using NPOI.SS.Formula.Functions;
using SomewhatEnhancedDisplay.Config;
using UnityEngine.UI;
using YKF;

namespace SomewhatEnhancedDisplay.UI.HoverGuide;

public class ModLayerConfigTabStyle : YKLayout<object>
{
    private int SelectedStyleIndex { get; set; }
    private ModConfigHoverGuideStyle SelectedStyle => Mod.Config.HoverGuide.Styles[SelectedStyleIndex];

    private static ModConfigHoverGuide Config => Mod.Config.HoverGuide;

    public override void OnLayout()
    {
        Spacer(12);

        var styleLayout = Horizontal().WithFitMode(ContentSizeFitter.FitMode.PreferredSize).WithPivot(0, 0.5f);
        var (selectStyleDropDown, updateSelectStyleDropDownList) = AddLabelDropDown(
            layout: styleLayout,
            label: ModConsts.SourceId.ConfigSelectStyleToEdit,
            init: 0,
            values: Config.Styles,
            labelFunc: (value, index) => $"{ModConsts.SourceId.ConfigStyle.lang()} {index + 1}",
            changedFunc: (index, Value) => SelectedStyleIndex = index,
            width: 180
        );  

        styleLayout.Spacer(0, 96);

        AddButton(
            layout: styleLayout,
            label: ModConsts.SourceId.ConfigAddStyle,
            clickedFunc: () =>
            {
                Config.Styles.Add(new());
                SelectedStyleIndex = Config.Styles.Count - 1;
                updateSelectStyleDropDownList(SelectedStyleIndex, Config.Styles);
            },
            width: 100
        );
        styleLayout.Spacer(0, 6);
        AddButton(
            layout: styleLayout,
            label: ModConsts.SourceId.ConfigDeleteStyle,
            clickedFunc: () =>
            {
                if (Config.Styles.Count <= 1)
                {
                    return;
                }

                Dialog.YesNo(ModConsts.SourceId.DialogDeleteStyle, () =>
                {
                    Config.Styles.RemoveAt(SelectedStyleIndex);
                    SelectedStyleIndex = 0;
                    updateSelectStyleDropDownList(SelectedStyleIndex, Config.Styles);
                });
            },
            width: 80
        );
    }

    private UIButton AddButton(YKLayout layout, string label, Action clickedFunc, int? width = null)
    {
        var button = layout.Button(label.lang(), clickedFunc);
        button.WithWidth(width ?? 150);
        return button;
    }

    private (UIDropdown, Action<int, IEnumerable<T>>) AddLabelDropDown<T>(YKLayout layout, string label, int init, IEnumerable<T> values, Func<T, int, string> labelFunc, Action<int, T> changedFunc, int? width = null)
    {
        layout.Text($"{label.lang()}:", FontColor.Header);
        layout.Spacer(0, 12);
        var valueList = values.ToList();
        var dropdown = layout.Dropdown([.. valueList.Select(labelFunc)], index => changedFunc(index, valueList[index]), init);
        dropdown.WithWidth(width ?? 180);
        return (dropdown, (index, values) => dropdown.SetList(index, [.. values], labelFunc, changedFunc));
    }
}

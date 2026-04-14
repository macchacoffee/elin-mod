using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using YKF;

namespace SomewhatEnhancedDisplay.Extensions;

public static class YKLayoutExtensions
{
    public static UIButton AddModButton(this YKLayout layout, string label, Action onClicked, int? width = null)
    {
        var button = layout.Button(label.lang(), onClicked);
        button.WithWidth(width ?? 150);
        return button;
    }

    public static UIButton AddModToggle(this YKLayout layout, string label, bool init, Action<bool> onChanged)
    {
        return layout.Toggle(label, init, onChanged);
    }

    public static (UIDropdown, Action<int, IEnumerable<T>>) AddModDropdown<T>(this YKLayout layout, string label, int init, IEnumerable<T> values, Func<T, int, string> getLabel, Action<int, T> onChanged, int? width = null)
    {
        layout.Text($"{label.lang()}:", FontColor.Header);
        layout.Spacer(0, 12);
        var valueList = values.ToList();
        var dropdown = layout.Dropdown([.. valueList.Select(getLabel)], index => onChanged(index, valueList[index]), init);
        dropdown.WithWidth(width ?? 180);
        return (dropdown, (index, values) => dropdown.SetList(index, [.. values], getLabel, onChanged));
    }

    public static Slider AddModSlider(this YKLayout layout, Func<float, string> getLabel, float init, float min, float max, float step, Action<float> onChanged, int? width = null)
    {
        if (step <= 0)
        {
            new ArgumentException("Step of slider must be more than 0");
        }

        var slider = layout.Slider(
            init / step,
            value => onChanged(value * step),
            min / step,
            max / step,
            value => getLabel(value * step)
        ).WithWidth(width ?? 200);
        slider.wholeNumbers = true;

        return slider;
    }

    public static UIButton AddModColorPicker(this YKLayout layout, string label, Color? init, Action<Color> onChanged)
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
                onChanged(color);
            });
        });

        return button;
    }
}
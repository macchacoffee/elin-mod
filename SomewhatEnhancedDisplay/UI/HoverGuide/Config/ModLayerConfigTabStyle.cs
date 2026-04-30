using System;
using SomewhatEnhancedDisplay.Config;
using SomewhatEnhancedDisplay.Extensions;
using UnityEngine.UI;
using YKF;

namespace SomewhatEnhancedDisplay.UI.HoverGuide.Config;

public class ModLayerConfigTabStyle : YKLayout<ModLayerConfigContext>
{
    private static ModConfigHoverGuide Config => Mod.Config.HoverGuide;

    private int SelectedStyleIndex
    {
        get => Layer.Data.SelectedStyleIndex;
        set => Layer.Data.SelectedStyleIndex = value;
    }

    public override void OnLayout()
    {
        Header(ModConsts.SourceId.ConfigEditStyle);
        Spacer(8);

        var headerLayout = Horizontal().WithFitMode(ContentSizeFitter.FitMode.PreferredSize).WithPivot(0, 0.5f);
        var (_, updateStyleDropdown) = headerLayout.AddModDropdown(
            label: ModConsts.SourceId.ConfigSelectStyleToEdit,
            init: 0,
            values: Config.Styles,
            getLabel: (value, index) => $"{ModConsts.SourceId.ConfigStyle.lang()} {index + 1}",
            onChanged: (index, Value) => SelectedStyleIndex = index,
            width: 180
        );

        headerLayout.Spacer(0, 40);

        // TODO 追加の選択肢 (全表示、表示多め、表示少なめなど) を増やす
        headerLayout.AddModButton(
            label: ModConsts.SourceId.ConfigAddStyle,
            onClicked: () =>
            {
                Config.Styles.Add(new());
                SelectedStyleIndex = Config.Styles.Count - 1;
                updateStyleDropdown(SelectedStyleIndex, Config.Styles);
            },
            width: 100
        );

        headerLayout.Spacer(0, 6);
        headerLayout.AddModButton(
            label: ModConsts.SourceId.ConfigDeleteStyle,
            onClicked: () =>
            {
                if (Config.Styles.Count <= 1)
                {
                    return;
                }
                var d = Dialog.YesNo(ModConsts.SourceId.DialogDeleteStyle, () =>
                {
                    Config.Styles.RemoveAt(SelectedStyleIndex);
                    SelectedStyleIndex = Math.Max(0, SelectedStyleIndex - 1);
                    updateStyleDropdown(SelectedStyleIndex, Config.Styles);
                });
            },
            width: 80
        );
    }
}

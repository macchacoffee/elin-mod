using UnityEngine.UI;

namespace SomewhatEnhancedDisplay.UI.HoverGuide.Config;

public class ModLayerConfigTabStyleTargetThing : ModLayerConfigTabStyleTarget
{
    protected override void OnLayoutInternal()
    {
        var styleEditLayout = Vertical();
        styleEditLayout.Fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var cellWidth = 200;
        var maxColumn = 3;
        var line = 0;

        styleEditLayout.Header(ModConsts.SourceId.ConfigDisplayItems);

        line++;
        EditStyleUI.AddToggles(
            layout: styleEditLayout,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.Lv,
                Init: SelectedStyle.Thing.DisplayLv,
                OnChanged: value => SelectedStyle.Thing.DisplayLv = value,
                GetConfig: () => SelectedStyle.Thing.DisplayLv
            ),
            new(
                Label: ModConsts.SourceId.UseRarityColor,
                Init: SelectedStyle.Thing.UseRarityColor,
                OnChanged: value => SelectedStyle.Thing.UseRarityColor = value,
                GetConfig: () => SelectedStyle.Thing.UseRarityColor
            )
        );

        line++;
        EditStyleUI.AddToggles(
            layout: styleEditLayout,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.Material,
                Init: SelectedStyle.Thing.DisplayMaterial,
                OnChanged: value => SelectedStyle.Thing.DisplayMaterial = value,
                GetConfig: () => SelectedStyle.Thing.DisplayMaterial
            ),
            new(
                Label: ModConsts.SourceId.LockLv,
                Init: SelectedStyle.Thing.DisplayLockLv,
                OnChanged: value => SelectedStyle.Thing.DisplayLockLv = value,
                GetConfig: () => SelectedStyle.Thing.DisplayLockLv
            ),
            new(
                Label: ModConsts.SourceId.Fressness,
                Init: SelectedStyle.Thing.DisplayFressness,
                OnChanged: value => SelectedStyle.Thing.DisplayFressness = value,
                GetConfig: () => SelectedStyle.Thing.DisplayFressness
            )
        );
    }
}

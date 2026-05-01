namespace SomewhatEnhancedDisplay.UI.HoverGuide.Config;

public class ModLayerConfigTabStyleTargetThing : ModLayerConfigTabStyleTarget
{
    protected override void OnLayoutInternal()
    {
        var cellWidth = 200;
        var maxColumn = 3;
        var line = 0;

        Header(ModConsts.SourceId.ConfigDisplayItems);

        line++;
        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Lv,
                Init: SelectedStyle.Thing.DisplayLv,
                OnChanged: value => SelectedStyle.Thing.DisplayLv = value,
                GetConfig: () => SelectedStyle.Thing.DisplayLv
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.UseRarityColor,
                Init: SelectedStyle.Thing.UseRarityColor,
                OnChanged: value => SelectedStyle.Thing.UseRarityColor = value,
                GetConfig: () => SelectedStyle.Thing.UseRarityColor
            )
        );

        line++;
        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Material,
                Init: SelectedStyle.Thing.DisplayMaterial,
                OnChanged: value => SelectedStyle.Thing.DisplayMaterial = value,
                GetConfig: () => SelectedStyle.Thing.DisplayMaterial
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.LockLv,
                Init: SelectedStyle.Thing.DisplayLockLv,
                OnChanged: value => SelectedStyle.Thing.DisplayLockLv = value,
                GetConfig: () => SelectedStyle.Thing.DisplayLockLv
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Fressness,
                Init: SelectedStyle.Thing.DisplayFressness,
                OnChanged: value => SelectedStyle.Thing.DisplayFressness = value,
                GetConfig: () => SelectedStyle.Thing.DisplayFressness
            )
        );
    }
}

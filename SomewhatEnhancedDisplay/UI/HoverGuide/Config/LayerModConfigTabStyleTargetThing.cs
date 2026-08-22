namespace Macchacoffee.ElinMods.SomewhatEnhancedDisplay.UI.HoverGuide.Config;

internal class LayerModConfigTabStyleTargetThing : LayerModConfigTabStyleTarget
{
    private const int _cellWidth1 = 200;
    private const int _maxColumn1 = 3;

    private const int _cellWidth3 = 400;
    private const int _maxColumn3 = 1;

    protected override void OnLayoutInternal()
    {
        Header(ModConsts.SourceId.ConfigDisplayItems);

        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.ThingName,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
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
                GetConfig: () => SelectedStyle.Thing.UseRarityColor,
                Tooltip: ModConsts.SourceId.TooltipUseRarityColor
            )
        );

        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.ThingExtraInformation,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
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
                GetConfig: () => SelectedStyle.Thing.DisplayFressness,
                Tooltip: ModConsts.SourceId.TooltipFressness
            )
        );

        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Others,
            cellWidth: _cellWidth3,
            maxColumn: _maxColumn3,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.DisplayUnidentifiedItemsAsIdentified,
                Init: SelectedStyle.Thing.DisplayUnidentifiedItemsAsIdentified,
                OnChanged: value => SelectedStyle.Thing.DisplayUnidentifiedItemsAsIdentified = value,
                GetConfig: () => SelectedStyle.Thing.DisplayUnidentifiedItemsAsIdentified,
                Tooltip: ModConsts.SourceId.TooltipDisplayUnidentifiedItemsAsIdentified
            )
        );
    }
}

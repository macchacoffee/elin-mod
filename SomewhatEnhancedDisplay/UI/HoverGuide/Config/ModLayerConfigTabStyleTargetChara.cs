using UnityEngine.UI;

namespace SomewhatEnhancedDisplay.UI.HoverGuide.Config;

public class ModLayerConfigTabStyleTargetChara : ModLayerConfigTabStyleTarget
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
                Label: ModConsts.SourceId.Type,
                Init: SelectedStyle.Chara.DisplayType,
                OnChanged: value => SelectedStyle.Chara.DisplayType = value,
                GetConfig: () => SelectedStyle.Chara.DisplayType
            ),
            new(
                Label: ModConsts.SourceId.Lv,
                Init: SelectedStyle.Chara.DisplayLv,
                OnChanged: value => SelectedStyle.Chara.DisplayLv = value,
                GetConfig: () => SelectedStyle.Chara.DisplayLv
            )
        );

        line++;

        line++;
        EditStyleUI.AddToggle(
            layout: styleEditLayout,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.HealthBar,
                Init: SelectedStyle.Chara.DisplayHealthBar,
                OnChanged: value => SelectedStyle.Chara.DisplayHealthBar = value,
                GetConfig: () => SelectedStyle.Chara.DisplayHealthBar
            )
        );

        line++;
        EditStyleUI.AddToggles(
            layout: styleEditLayout,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.Gender,
                Init: SelectedStyle.Chara.DisplayGender,
                OnChanged: value => SelectedStyle.Chara.DisplayGender = value,
                GetConfig: () => SelectedStyle.Chara.DisplayGender
            ),
            new(
                Label: ModConsts.SourceId.Age,
                Init: SelectedStyle.Chara.DisplayAge,
                OnChanged: value => SelectedStyle.Chara.DisplayAge = value,
                GetConfig: () => SelectedStyle.Chara.DisplayAge
            ),
            new(
                Label: ModConsts.SourceId.Race,
                Init: SelectedStyle.Chara.DisplayRace,
                OnChanged: value => SelectedStyle.Chara.DisplayRace = value,
                GetConfig: () => SelectedStyle.Chara.DisplayRace
            ),
            new(
                Label: ModConsts.SourceId.JobTactics,
                Init: SelectedStyle.Chara.DisplayJobTactics,
                OnChanged: value => SelectedStyle.Chara.DisplayJobTactics = value,
                GetConfig: () => SelectedStyle.Chara.DisplayJobTactics
            ),
            new(
                Label: ModConsts.SourceId.Hobby,
                Init: SelectedStyle.Chara.DisplayHobby,
                OnChanged: value => SelectedStyle.Chara.DisplayHobby = value,
                GetConfig: () => SelectedStyle.Chara.DisplayHobby
            )
        );

        line++;
        EditStyleUI.AddToggles(
            layout: styleEditLayout,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.Affinity,
                Init: SelectedStyle.Chara.DisplayAffinity,
                OnChanged: value => SelectedStyle.Chara.DisplayAffinity = value,
                GetConfig: () => SelectedStyle.Chara.DisplayAffinity
            ),
            new(
                Label: ModConsts.SourceId.Favorite,
                Init: SelectedStyle.Chara.DisplayFavorite,
                OnChanged: value => SelectedStyle.Chara.DisplayFavorite = value,
                GetConfig: () => SelectedStyle.Chara.DisplayFavorite
            )
        );

        line++;
        EditStyleUI.AddToggles(
            layout: styleEditLayout,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.HP,
                Init: SelectedStyle.Chara.DisplayHP,
                OnChanged: value => SelectedStyle.Chara.DisplayHP = value,
                GetConfig: () => SelectedStyle.Chara.DisplayHP
            ),
            new(
                Label: ModConsts.SourceId.Mana,
                Init: SelectedStyle.Chara.DisplayMana,
                OnChanged: value => SelectedStyle.Chara.DisplayMana = value,
                GetConfig: () => SelectedStyle.Chara.DisplayMana
            ),
            new(
                Label: ModConsts.SourceId.Stamina,
                Init: SelectedStyle.Chara.DisplayStamina,
                OnChanged: value => SelectedStyle.Chara.DisplayStamina = value,
                GetConfig: () => SelectedStyle.Chara.DisplayStamina
            )
        );

        line++;
        EditStyleUI.AddToggles(
            layout: styleEditLayout,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.DVPV,
                Init: SelectedStyle.Chara.DisplayDVPV,
                OnChanged: value => SelectedStyle.Chara.DisplayDVPV = value,
                GetConfig: () => SelectedStyle.Chara.DisplayDVPV
            ),
            new(
                Label: ModConsts.SourceId.Speed,
                Init: SelectedStyle.Chara.DisplaySpeed,
                OnChanged: value => SelectedStyle.Chara.DisplaySpeed = value,
                GetConfig: () => SelectedStyle.Chara.DisplaySpeed
            ),
            new(
                Label: ModConsts.SourceId.Exp,
                Init: SelectedStyle.Chara.DisplayExp,
                OnChanged: value => SelectedStyle.Chara.DisplayExp = value,
                GetConfig: () => SelectedStyle.Chara.DisplayExp
            ),
            new(
                Label: ModConsts.SourceId.MainElement,
                Init: SelectedStyle.Chara.DisplayMainElement,
                OnChanged: value => SelectedStyle.Chara.DisplayMainElement = value,
                GetConfig: () => SelectedStyle.Chara.DisplayMainElement
            )
        );

        line++;
        EditStyleUI.AddToggle(
            layout: styleEditLayout,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.PrimaryAttributes,
                Init: SelectedStyle.Chara.DisplayPrimaryAttributes,
                OnChanged: value => SelectedStyle.Chara.DisplayPrimaryAttributes = value,
                GetConfig: () => SelectedStyle.Chara.DisplayPrimaryAttributes
            )
        );

        line++;
        EditStyleUI.AddToggles(
            layout: styleEditLayout,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.Feat,
                Init: SelectedStyle.Chara.DisplayFeat,
                OnChanged: value => SelectedStyle.Chara.DisplayFeat = value,
                GetConfig: () => SelectedStyle.Chara.DisplayFeat
            ),
            new(
                Label: ModConsts.SourceId.FeatValue,
                Init: SelectedStyle.Chara.DisplayFeatValue,
                OnChanged: value => SelectedStyle.Chara.DisplayFeatValue = value,
                GetConfig: () => SelectedStyle.Chara.DisplayFeatValue
            )
        );

        line++;
        EditStyleUI.AddToggles(
            layout: styleEditLayout,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.Act,
                Init: SelectedStyle.Chara.DisplayAct,
                OnChanged: value => SelectedStyle.Chara.DisplayAct = value,
                GetConfig: () => SelectedStyle.Chara.DisplayAct
            ),
            new(
                Label: ModConsts.SourceId.ActParty,
                Init: SelectedStyle.Chara.DisplayActParty,
                OnChanged: value => SelectedStyle.Chara.DisplayActParty = value,
                GetConfig: () => SelectedStyle.Chara.DisplayActParty
            )
        );

        line++;
        EditStyleUI.AddToggles(
            layout: styleEditLayout,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.Resist,
                Init: SelectedStyle.Chara.DisplayResist,
                OnChanged: value => SelectedStyle.Chara.DisplayResist = value,
                GetConfig: () => SelectedStyle.Chara.DisplayResist
            ),
            new(
                Label: ModConsts.SourceId.ResistValue,
                Init: SelectedStyle.Chara.DisplayResistValue,
                OnChanged: value => SelectedStyle.Chara.DisplayResistValue = value,
                GetConfig: () => SelectedStyle.Chara.DisplayResistValue
            )
        );

        line++;
        EditStyleUI.AddToggles(
            layout: styleEditLayout,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.Stats,
                Init: SelectedStyle.Chara.DisplayStats,
                OnChanged: value => SelectedStyle.Chara.DisplayStats = value,
                GetConfig: () => SelectedStyle.Chara.DisplayStats
            ),
            new(
                Label: ModConsts.SourceId.StatsValue,
                Init: SelectedStyle.Chara.DisplayStatsValue,
                OnChanged: value => SelectedStyle.Chara.DisplayStatsValue = value,
                GetConfig: () => SelectedStyle.Chara.DisplayStatsValue
            )
        );

        styleEditLayout.Spacer(20);
        styleEditLayout.Header(ModConsts.SourceId.ConfigDisplayTransmutation);

        EditStyleUI.AddToggles(
            layout: styleEditLayout,
            headerLabel: null,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.EnableShadowform,
                Init: SelectedStyle.Chara.EnableShadowform,
                OnChanged: value => SelectedStyle.Chara.EnableShadowform = value,
                GetConfig: () => SelectedStyle.Chara.EnableShadowform
            ),
            new(
                Label: ModConsts.SourceId.EnableMimicry,
                Init: SelectedStyle.Chara.EnableMimicry,
                OnChanged: value => SelectedStyle.Chara.EnableMimicry = value,
                GetConfig: () => SelectedStyle.Chara.EnableMimicry
            )
        );

        styleEditLayout.Spacer(20);
        styleEditLayout.Header(ModConsts.SourceId.HealthBar);

        EditStyleUI.AddToggles(
            layout: styleEditLayout,
            headerLabel: null,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.DisplayValue,
                Init: SelectedStyle.Chara.HealthBar.DisplayValue,
                OnChanged: value => SelectedStyle.Chara.HealthBar.DisplayValue = value,
                GetConfig: () => SelectedStyle.Chara.HealthBar.DisplayValue
            ),
            new(
                Label: ModConsts.SourceId.UseAnimation,
                Init: SelectedStyle.Chara.HealthBar.UseAnimation,
                OnChanged: value => SelectedStyle.Chara.HealthBar.UseAnimation = value,
                GetConfig: () => SelectedStyle.Chara.HealthBar.UseAnimation
            )
        );
    }
}

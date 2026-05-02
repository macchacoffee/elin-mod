using System;
using System.Collections.Generic;
using SomewhatEnhancedDisplay.Config;
using YKF;

namespace SomewhatEnhancedDisplay.UI.HoverGuide.Config;

public class ModLayerConfigTabStyleTargetChara : ModLayerConfigTabStyleTarget
{
    private static readonly Dictionary<ModHealthBarDisplayTarget, string> HealthBarDisplayTargetIdLangs = new() {
        {ModHealthBarDisplayTarget.None, ModConsts.SourceId.TargetNone},
        {ModHealthBarDisplayTarget.Boss, ModConsts.SourceId.TargetBoss},
        {ModHealthBarDisplayTarget.Elite, ModConsts.SourceId.TargetElite},
        {ModHealthBarDisplayTarget.All, ModConsts.SourceId.TargetAll},
    };
    private static readonly List<ModHealthBarDisplayTarget> HealthBarDisplayTargets = [.. HealthBarDisplayTargetIdLangs.Keys];

    private ModConfigHoverGuideStyleChara Config => SelectedStyle.Chara;

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
                Label: ModConsts.SourceId.Type,
                Init: Config.DisplayType,
                OnChanged: value => Config.DisplayType = value,
                GetConfig: () => Config.DisplayType
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Lv,
                Init: Config.DisplayLv,
                OnChanged: value => Config.DisplayLv = value,
                GetConfig: () => Config.DisplayLv
            )
        );

        line++;

        line++;
        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.HealthBar,
                Init: Config.DisplayHealthBar,
                OnChanged: value => Config.DisplayHealthBar = value,
                GetConfig: () => Config.DisplayHealthBar
            )
        );

        line++;
        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Gender,
                Init: Config.DisplayGender,
                OnChanged: value => Config.DisplayGender = value,
                GetConfig: () => Config.DisplayGender
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Age,
                Init: Config.DisplayAge,
                OnChanged: value => Config.DisplayAge = value,
                GetConfig: () => Config.DisplayAge
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Race,
                Init: Config.DisplayRace,
                OnChanged: value => Config.DisplayRace = value,
                GetConfig: () => Config.DisplayRace
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.JobTactics,
                Init: Config.DisplayJobTactics,
                OnChanged: value => Config.DisplayJobTactics = value,
                GetConfig: () => Config.DisplayJobTactics
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Hobby,
                Init: Config.DisplayHobby,
                OnChanged: value => Config.DisplayHobby = value,
                GetConfig: () => Config.DisplayHobby
            )
        );

        line++;
        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Affinity,
                Init: Config.DisplayAffinity,
                OnChanged: value => Config.DisplayAffinity = value,
                GetConfig: () => Config.DisplayAffinity
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Favorite,
                Init: Config.DisplayFavorite,
                OnChanged: value => Config.DisplayFavorite = value,
                GetConfig: () => Config.DisplayFavorite
            )
        );

        line++;
        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.HP,
                Init: Config.DisplayHP,
                OnChanged: value => Config.DisplayHP = value,
                GetConfig: () => Config.DisplayHP
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Mana,
                Init: Config.DisplayMana,
                OnChanged: value => Config.DisplayMana = value,
                GetConfig: () => Config.DisplayMana
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Stamina,
                Init: Config.DisplayStamina,
                OnChanged: value => Config.DisplayStamina = value,
                GetConfig: () => Config.DisplayStamina
            )
        );

        line++;
        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.DVPV,
                Init: Config.DisplayDVPV,
                OnChanged: value => Config.DisplayDVPV = value,
                GetConfig: () => Config.DisplayDVPV
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Speed,
                Init: Config.DisplaySpeed,
                OnChanged: value => Config.DisplaySpeed = value,
                GetConfig: () => Config.DisplaySpeed
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Exp,
                Init: Config.DisplayExp,
                OnChanged: value => Config.DisplayExp = value,
                GetConfig: () => Config.DisplayExp
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.MainElement,
                Init: Config.DisplayMainElement,
                OnChanged: value => Config.DisplayMainElement = value,
                GetConfig: () => Config.DisplayMainElement
            )
        );

        line++;
        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.PrimaryAttributes,
                Init: Config.DisplayPrimaryAttributes,
                OnChanged: value => Config.DisplayPrimaryAttributes = value,
                GetConfig: () => Config.DisplayPrimaryAttributes
            )
        );

        line++;
        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: 2,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Feat,
                Init: Config.DisplayFeat,
                OnChanged: value => Config.DisplayFeat = value,
                GetConfig: () => Config.DisplayFeat
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.FeatValue,
                Init: Config.DisplayFeatValue,
                OnChanged: value => Config.DisplayFeatValue = value,
                GetConfig: () => Config.DisplayFeatValue
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.WrapLine,
                Init: Config.FeatLineWrapping.Enable,
                OnChanged: value => Config.FeatLineWrapping.Enable = value,
                GetConfig: () => Config.FeatLineWrapping.Enable
            ),
            new EditStyleSliderUIItem(
                GetLabel: value => $"{ModConsts.SourceId.MaxItemsPerLine.lang()}({value})",
                Init: Config.FeatLineWrapping.MaxItemsPerLine,
                Min: 1,
                Max: 20,
                Step: 1,
                OnChanged: value => Config.FeatLineWrapping.MaxItemsPerLine = (int)value,
                GetConfig: () => Config.FeatLineWrapping.MaxItemsPerLine
            )
        );

        line++;
        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: 2,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Act,
                Init: Config.DisplayAct,
                OnChanged: value => Config.DisplayAct = value,
                GetConfig: () => Config.DisplayAct
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.ActParty,
                Init: Config.DisplayActParty,
                OnChanged: value => Config.DisplayActParty = value,
                GetConfig: () => Config.DisplayActParty
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.WrapLine,
                Init: Config.ActLineWrapping.Enable,
                OnChanged: value => Config.ActLineWrapping.Enable = value,
                GetConfig: () => Config.ActLineWrapping.Enable
            ),
            new EditStyleSliderUIItem(
                GetLabel: value => $"{ModConsts.SourceId.MaxItemsPerLine.lang()}({value})",
                Init: Config.ActLineWrapping.MaxItemsPerLine,
                Min: 1,
                Max: 20,
                Step: 1,
                OnChanged: value => Config.ActLineWrapping.MaxItemsPerLine = (int)value,
                GetConfig: () => Config.ActLineWrapping.MaxItemsPerLine
            )
        );

        line++;
        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Resist,
                Init: Config.DisplayResist,
                OnChanged: value => Config.DisplayResist = value,
                GetConfig: () => Config.DisplayResist
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.ResistValue,
                Init: Config.DisplayResistValue,
                OnChanged: value => Config.DisplayResistValue = value,
                GetConfig: () => Config.DisplayResistValue
            )
        );
        EditStyleUI.Add(
            layout: this,
            headerLabel: null,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.GroupResistByLavel,
                Init: Config.GroupResistByLavel,
                OnChanged: value => Config.GroupResistByLavel = value,
                GetConfig: () => Config.GroupResistByLavel
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.UseShortResistLavelLabel,
                Init: Config.UseShortResistLavelLabel,
                OnChanged: value => Config.UseShortResistLavelLabel = value,
                GetConfig: () => Config.UseShortResistLavelLabel
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.DisplayNoneResistLevel,
                Init: Config.DisplayNoneResistLevel,
                OnChanged: value => Config.DisplayNoneResistLevel = value,
                GetConfig: () => Config.DisplayNoneResistLevel
            )
        );
        EditStyleUI.Add(
            layout: this,
            headerLabel: null,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.WrapLine,
                Init: Config.ResistLineWrapping.Enable,
                OnChanged: value => Config.ResistLineWrapping.Enable = value,
                GetConfig: () => Config.ResistLineWrapping.Enable
            ),
            new EditStyleSliderUIItem(
                GetLabel: value => $"{ModConsts.SourceId.MaxItemsPerLine.lang()}({value})",
                Init: Config.ResistLineWrapping.MaxItemsPerLine,
                Min: 1,
                Max: 20,
                Step: 1,
                OnChanged: value => Config.ResistLineWrapping.MaxItemsPerLine = (int)value,
                GetConfig: () => Config.ResistLineWrapping.MaxItemsPerLine
            )
        );

        line++;
        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: 2,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Stats,
                Init: Config.DisplayStats,
                OnChanged: value => Config.DisplayStats = value,
                GetConfig: () => Config.DisplayStats
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.StatsValue,
                Init: Config.DisplayStatsValue,
                OnChanged: value => Config.DisplayStatsValue = value,
                GetConfig: () => Config.DisplayStatsValue
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.WrapLine,
                Init: Config.StatsLineWrapping.Enable,
                OnChanged: value => Config.StatsLineWrapping.Enable = value,
                GetConfig: () => Config.StatsLineWrapping.Enable
            ),
            new EditStyleSliderUIItem(
                GetLabel: value => $"{ModConsts.SourceId.MaxItemsPerLine.lang()}({value})",
                Init: Config.StatsLineWrapping.MaxItemsPerLine,
                Min: 1,
                Max: 20,
                Step: 1,
                OnChanged: value => Config.StatsLineWrapping.MaxItemsPerLine = (int)value,
                GetConfig: () => Config.StatsLineWrapping.MaxItemsPerLine
            )
        );

        Spacer(20);
        Header(ModConsts.SourceId.ConfigTransmutation);

        EditStyleUI.Add(
            layout: this,
            headerLabel: null,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.EnableShadowform,
                Init: Config.EnableShadowform,
                OnChanged: value => Config.EnableShadowform = value,
                GetConfig: () => Config.EnableShadowform
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.EnableMimicry,
                Init: Config.EnableMimicry,
                OnChanged: value => Config.EnableMimicry = value,
                GetConfig: () => Config.EnableMimicry
            )
        );

        Spacer(20);
        Header(ModConsts.SourceId.HealthBar);

        EditStyleUI.Add(
            layout: this,
            headerLabel: null,
            cellWidth: cellWidth,
            maxColumn: 2,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.DisplayValue,
                Init: Config.HealthBar.DisplayValue,
                OnChanged: value => Config.HealthBar.DisplayValue = value,
                GetConfig: () => Config.HealthBar.DisplayValue
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.UseAnimation,
                Init: Config.HealthBar.UseAnimation,
                OnChanged: value => Config.HealthBar.UseAnimation = value,
                GetConfig: () => Config.HealthBar.UseAnimation
            ),
            new EditStyleSliderUIItem(
                GetLabel: value => $"{ModConsts.SourceId.Width.lang()}({value})",
                Init: Config.HealthBar.Width,
                Min: 200,
                Max: 800,
                Step: 10,
                OnChanged: value =>
                {
                    Plugin.LogInfo($"OnChanged {Config.HealthBar.Width} {value}");
                    Config.HealthBar.Width = (int)value;
                },
                GetConfig: () => Config.HealthBar.Width
            )
        );

        AddHealthBarDisplay(
            layout: this,
            getConfig: () => Config.HealthBar.DisplayForEnemy,
            headerLabel: ModConsts.SourceId.Enemy,
            cellWidth: cellWidth,
            maxColumn: maxColumn
        );
        AddHealthBarDisplay(
            layout: this,
            getConfig: () => Config.HealthBar.DisplayForNetural,
            headerLabel: ModConsts.SourceId.Netural,
            cellWidth: cellWidth,
            maxColumn: maxColumn
        );
        AddHealthBarDisplay(
            layout: this,
            getConfig: () =>  Config.HealthBar.DisplayForFriend,
            headerLabel: ModConsts.SourceId.Friend,
            cellWidth: cellWidth,
            maxColumn: maxColumn
        );
        AddHealthBarDisplay(
            layout: this,
            getConfig: () => Config.HealthBar.DisplayForAlly,
            headerLabel: ModConsts.SourceId.Ally,
            cellWidth: cellWidth,
            maxColumn: maxColumn
        );
    }

    private void AddHealthBarDisplay(YKLayout layout, Func<ModConfigHealthBarDisplay> getConfig, string headerLabel, int cellWidth, int maxColumn)
    {
        EditStyleUI.Add(
            layout: layout,
            headerLabel: headerLabel,
            cellWidth: (int)(cellWidth * 1.2),
            maxColumn: 1,
            new EditStyleDropdownUIItem<ModHealthBarDisplayTarget>(
                Label: ModConsts.SourceId.Target,
                Init: HealthBarDisplayTargets.IndexOf(getConfig().Target),
                Values: HealthBarDisplayTargets,
                GetLabel: (_, value) => HealthBarDisplayTargetIdLangs[value].lang(),
                OnChanged: (_, value) => getConfig().Target = value,
                GetConfig: () => (HealthBarDisplayTargets.IndexOf(getConfig().Target), HealthBarDisplayTargets)
            )
        );
        EditStyleUI.Add(
            layout: layout,
            headerLabel: null,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.NotInCombat,
                Init: getConfig().NotInCombat,
                OnChanged: value => getConfig().NotInCombat = value,
                GetConfig: () => getConfig().NotInCombat
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.InFullHealth,
                Init: getConfig().InFullHealth,
                OnChanged: value => getConfig().InFullHealth = value,
                GetConfig: () => getConfig().InFullHealth
            )
        );
    }
}

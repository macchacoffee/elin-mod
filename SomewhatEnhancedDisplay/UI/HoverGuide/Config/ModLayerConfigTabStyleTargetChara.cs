using System;
using System.Collections.Generic;
using SomewhatEnhancedDisplay.Config;
using YKF;

namespace SomewhatEnhancedDisplay.UI.HoverGuide.Config;

internal class ModLayerConfigTabStyleTargetChara : ModLayerConfigTabStyleTarget
{
    private const int _cellWidth1 = 200;
    private const int _maxColumn1 = 3;

    private const int _cellWidth2 = 300;
    private const int _maxColumn2 = 2;

    private static readonly Dictionary<ModHealthBarDisplayTarget, string> _healthBarDisplayTargetIdLangs = new() {
        {ModHealthBarDisplayTarget.None, ModConsts.SourceId.TargetNone},
        {ModHealthBarDisplayTarget.Boss, ModConsts.SourceId.TargetBoss},
        {ModHealthBarDisplayTarget.Elite, ModConsts.SourceId.TargetElite},
        {ModHealthBarDisplayTarget.All, ModConsts.SourceId.TargetAll},
    };
    private static readonly List<ModHealthBarDisplayTarget> _healthBarDisplayTargets = [.. _healthBarDisplayTargetIdLangs.Keys];

    private ModConfigHoverGuideStyleChara Config => SelectedStyle.Chara;

    protected override void OnLayoutInternal()
    {
        Header(ModConsts.SourceId.ConfigDisplayItems);

        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.CharaName,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Type,
                Init: Config.DisplayType,
                OnChanged: value => Config.DisplayType = value,
                GetConfig: () => Config.DisplayType,
                Tooltip: ModConsts.SourceId.TooltipCharaType
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Lv,
                Init: Config.DisplayLv,
                OnChanged: value => Config.DisplayLv = value,
                GetConfig: () => Config.DisplayLv
            )
        );

        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.CharaExtraInformation,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.LvComparison,
                Init: Config.DisplayLvComparison,
                OnChanged: value => Config.DisplayLvComparison = value,
                GetConfig: () => Config.DisplayLvComparison
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.FactionMemberType,
                Init: Config.DisplayFactionMemberType,
                OnChanged: value => Config.DisplayFactionMemberType = value,
                GetConfig: () => Config.DisplayFactionMemberType,
                Tooltip: ModConsts.SourceId.TooltipFactionMemberType
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.HeightDifference,
                Init: Config.DisplayHeightDifference,
                OnChanged: value => Config.DisplayHeightDifference = value,
                GetConfig: () => Config.DisplayHeightDifference
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.MilkBaby,
                Init: Config.DisplayMilkBaby,
                OnChanged: value => Config.DisplayMilkBaby = value,
                GetConfig: () => Config.DisplayMilkBaby
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.Bounty,
                Init: Config.DisplayBounty,
                OnChanged: value => Config.DisplayBounty = value,
                GetConfig: () => Config.DisplayBounty
            )
        );

        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Faith,
            cellWidth: _cellWidth2,
            maxColumn: _maxColumn2,
            CreateItemDisplayModeDropdownUIItem(
                onChanged: value => Config.DisplayFaith = value,
                getConfig: () => Config.DisplayFaith
            )
        );

        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.BloodTaste,
            cellWidth: _cellWidth2,
            maxColumn: _maxColumn2,
            CreateItemDisplayModeDropdownUIItem(
                onChanged: value => Config.DisplayBloodTaste = value,
                getConfig: () => Config.DisplayBloodTaste
            )
        );

        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.HealthBar,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.HealthBar,
                Init: Config.DisplayHealthBar,
                OnChanged: value => Config.DisplayHealthBar = value,
                GetConfig: () => Config.DisplayHealthBar
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.HealthBarValue,
                Init: Config.HealthBar.DisplayValue,
                OnChanged: value => Config.HealthBar.DisplayValue = value,
                GetConfig: () => Config.HealthBar.DisplayValue
            )
        );
        EditStyleUI.Add(
            layout: this,
            headerLabel: null,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.UseAnimation,
                Init: Config.HealthBar.UseAnimation,
                OnChanged: value => Config.HealthBar.UseAnimation = value,
                GetConfig: () => Config.HealthBar.UseAnimation,
                Tooltip: ModConsts.SourceId.TooltipUseAnimation
            )
        );
        EditStyleUI.Add(
            layout: this,
            headerLabel: null,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
            new EditStyleSliderUIItem(
                GetLabel: value => $"{ModConsts.SourceId.HealthBarWidth.lang()}({value})",
                Init: Config.HealthBar.Width,
                Min: 200,
                Max: 800,
                Step: 10,
                OnChanged: value => Config.HealthBar.Width = (int)value,
                GetConfig: () => Config.HealthBar.Width
            )
        );
        AddHealthBarDisplay(
            layout: this,
            getConfig: () => Config.HealthBar.DisplayForEnemy,
            headerLabel: ModConsts.SourceId.Enemy,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1
        );
        AddHealthBarDisplay(
            layout: this,
            getConfig: () => Config.HealthBar.DisplayForNetural,
            headerLabel: ModConsts.SourceId.Netural,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1
        );
        AddHealthBarDisplay(
            layout: this,
            getConfig: () => Config.HealthBar.DisplayForFriend,
            headerLabel: ModConsts.SourceId.Friend,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1
        );
        AddHealthBarDisplay(
            layout: this,
            getConfig: () => Config.HealthBar.DisplayForAlly,
            headerLabel: ModConsts.SourceId.Ally,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1
        );


        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Profile,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
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
                Label: ModConsts.SourceId.Affinity,
                Init: Config.DisplayAffinity,
                OnChanged: value => Config.DisplayAffinity = value,
                GetConfig: () => Config.DisplayAffinity
            )
        );

        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Hobby,
            cellWidth: _cellWidth2,
            maxColumn: _maxColumn2,
            CreateItemDisplayModeDropdownUIItem(
                onChanged: value => Config.DisplayHobby = value,
                getConfig: () => Config.DisplayHobby
            )
        );

        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Favorite,
            cellWidth: _cellWidth2,
            maxColumn: _maxColumn2,
            CreateItemDisplayModeDropdownUIItem(
                onChanged: value => Config.DisplayFavorite = value,
                getConfig: () => Config.DisplayFavorite
            )
        );

        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.StatusAttributes1,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
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

        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.StatusAttributes2Others,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
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
                Label: ModConsts.SourceId.Luck,
                Init: Config.DisplayLuck,
                OnChanged: value => Config.DisplayLuck = value,
                GetConfig: () => Config.DisplayLuck
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
        EditStyleUI.Add(
            layout: this,
            headerLabel: null,
            cellWidth: _cellWidth2,
            maxColumn: _maxColumn2,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.DisplayExpForOnlyAlly,
                Init: Config.DisplayExpForOnlyAlly,
                OnChanged: value => Config.DisplayExpForOnlyAlly = value,
                GetConfig: () => Config.DisplayExpForOnlyAlly
            )
        );

        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.PrimaryAttributes,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.PrimaryAttributes,
                Init: Config.DisplayPrimaryAttributes,
                OnChanged: value => Config.DisplayPrimaryAttributes = value,
                GetConfig: () => Config.DisplayPrimaryAttributes
            )
        );

        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Feat,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
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
                GetConfig: () => Config.DisplayFeatValue,
                Tooltip: ModConsts.SourceId.TooltipFeatValue
            )
        );
        EditStyleUI.Add(
            layout: this,
            headerLabel: null,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.WrapLine,
                Init: Config.FeatLineWrapping.Enable,
                OnChanged: value => Config.FeatLineWrapping.Enable = value,
                GetConfig: () => Config.FeatLineWrapping.Enable
            )
        );
        EditStyleUI.Add(
            layout: this,
            headerLabel: null,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
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

        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Act,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
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
                GetConfig: () => Config.DisplayActParty,
                Tooltip: ModConsts.SourceId.TooltipActParty
            )
        );
        EditStyleUI.Add(
            layout: this,
            headerLabel: null,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.WrapLine,
                Init: Config.ActLineWrapping.Enable,
                OnChanged: value => Config.ActLineWrapping.Enable = value,
                GetConfig: () => Config.ActLineWrapping.Enable
            )
        );
        EditStyleUI.Add(
            layout: this,
            headerLabel: null,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
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

        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Resist,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
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
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.GroupResistByLevel,
                Init: Config.GroupResistByLavel,
                OnChanged: value => Config.GroupResistByLavel = value,
                GetConfig: () => Config.GroupResistByLavel,
                Tooltip: ModConsts.SourceId.TooltipGroupResistByLevel
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.UseShortResistLavelLabel,
                Init: Config.UseShortResistLavelLabel,
                OnChanged: value => Config.UseShortResistLavelLabel = value,
                GetConfig: () => Config.UseShortResistLavelLabel,
                Tooltip: ModConsts.SourceId.TooltipUseShortResistLavelLabel
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.DisplayNoneResistLevel,
                Init: Config.DisplayNoneResistLevel,
                OnChanged: value => Config.DisplayNoneResistLevel = value,
                GetConfig: () => Config.DisplayNoneResistLevel,
                Tooltip: ModConsts.SourceId.TooltipDisplayNoneResistLevel
            )
        );
        EditStyleUI.Add(
            layout: this,
            headerLabel: null,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.WrapLine,
                Init: Config.ResistLineWrapping.Enable,
                OnChanged: value => Config.ResistLineWrapping.Enable = value,
                GetConfig: () => Config.ResistLineWrapping.Enable
            )
        );
        EditStyleUI.Add(
            layout: this,
            headerLabel: null,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
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

        EditStyleUI.Add(
            layout: this,
            headerLabel: ModConsts.SourceId.Stats,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
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
            )
        );
        EditStyleUI.Add(
            layout: this,
            headerLabel: null,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.WrapLine,
                Init: Config.StatsLineWrapping.Enable,
                OnChanged: value => Config.StatsLineWrapping.Enable = value,
                GetConfig: () => Config.StatsLineWrapping.Enable
            )
        );
        EditStyleUI.Add(
            layout: this,
            headerLabel: null,
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
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
            cellWidth: _cellWidth1,
            maxColumn: _maxColumn1,
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.DisableShadowform,
                Init: Config.DisableShadowform,
                OnChanged: value => Config.DisableShadowform = value,
                GetConfig: () => Config.DisableShadowform,
                Tooltip: ModConsts.SourceId.TooltipDisableShadowform
            ),
            new EditStyleToogleUIItem(
                Label: ModConsts.SourceId.DisableMimicry,
                Init: Config.DisableMimicry,
                OnChanged: value => Config.DisableMimicry = value,
                GetConfig: () => Config.DisableMimicry,
                Tooltip: ModConsts.SourceId.TooltipDisableMimicry
            )
        );
    }

    private void AddHealthBarDisplay(YKLayout layout, Func<ModConfigHealthBarDisplay> getConfig, string headerLabel, int cellWidth, int maxColumn)
    {
        EditStyleUI.Add(
            layout: layout,
            headerLabel: $"{ModConsts.SourceId.HealthBar.lang()} ({headerLabel.lang()})",
            cellWidth: (int)(cellWidth * 1.2),
            maxColumn: 1,
            new EditStyleDropdownUIItem<ModHealthBarDisplayTarget>(
                Label: null,
                Init: _healthBarDisplayTargets.IndexOf(getConfig().Target),
                Values: _healthBarDisplayTargets,
                GetLabel: (_, value) => _healthBarDisplayTargetIdLangs[value].lang(),
                OnChanged: (_, value) => getConfig().Target = value,
                GetConfig: () => (_healthBarDisplayTargets.IndexOf(getConfig().Target), _healthBarDisplayTargets)
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

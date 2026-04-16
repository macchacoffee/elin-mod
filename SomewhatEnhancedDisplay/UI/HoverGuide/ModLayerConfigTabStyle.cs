using System;
using System.Collections.Generic;
using SomewhatEnhancedDisplay.Config;
using SomewhatEnhancedDisplay.Extensions;
using UnityEngine.UI;
using UnityEngine.UIElements;
using YKF;

namespace SomewhatEnhancedDisplay.UI.HoverGuide;

public class ModLayerConfigTabStyle : YKLayout<object>
{
    private int SelectedStyleIndex
    {
        get;
        set
        {
            field = value;
            EditStyleUI?.OnStyleChanged();
        }
    } = 0;

    private static ModConfigHoverGuide Config => Mod.Config.HoverGuide;
    private ModConfigHoverGuideStyle SelectedStyle => Config.Styles[SelectedStyleIndex];

    private EditStyleUIManager? EditStyleUI { get; set; }

    public override void OnLayout()
    {
        Spacer(12);

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

        var styleEditLayout = Vertical();
        styleEditLayout.Fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        EditStyleUI = new();
        var cellWidth = 200;
        var maxColumn = 3;
        var line = 0;

        styleEditLayout.Spacer(20);
        styleEditLayout.Header(ModConsts.SourceId.ConfigDisplayItems.lang(ModConsts.SourceId.Chara.lang()));

        line++;
        EditStyleUI.AddToggle(
            layout: styleEditLayout,
            headerLabel: ModConsts.SourceId.Line.lang(line.ToString()),
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.Lv,
                Init: SelectedStyle.Chara.DisplayLv,
                OnChanged: value => SelectedStyle.Chara.DisplayLv = value,
                GetConfig: () => SelectedStyle.Chara.DisplayLv
            )
        );

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

        EditStyleUI.AddToggle(
            layout: styleEditLayout,
            headerLabel: ModConsts.SourceId.Others,
            cellWidth: cellWidth,
            maxColumn: maxColumn,
            new(
                Label: ModConsts.SourceId.EnableMimicry,
                Init: SelectedStyle.Chara.EnableMimicry,
                OnChanged: value => SelectedStyle.Chara.EnableMimicry = value,
                GetConfig: () => SelectedStyle.Chara.EnableMimicry
            )
        );

        styleEditLayout.Spacer(20);
        styleEditLayout.Header(ModConsts.SourceId.HealthBar.lang());

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

        line = 0;

        styleEditLayout.Spacer(20);
        styleEditLayout.Header(ModConsts.SourceId.ConfigDisplayItems.lang(ModConsts.SourceId.Thing.lang()));

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

    private record EditStyleToogleUIItem(string Label, bool Init, Action<bool> OnChanged, Func<bool> GetConfig, string? Tooltip = null);

    private class EditStyleUIManager
    {
        private List<Action> StyleChangedFuncs { get; } = [];

        public void OnStyleChanged()
        {
            foreach (var func in StyleChangedFuncs)
            {
                func();
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
                StyleChangedFuncs.Add(() => toogle.SetCheck(item.GetConfig()));
            }
        }
    }
}

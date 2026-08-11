using System;
using YKF;
using NoPCC.Config;
using NoPCC.Mod;

namespace NoPCC.UI;

public class ModConfigMainTab : YKLayout<object>
{
    public override void OnLayout()
    {
        Header(ModConsts.SourceId.GeneralSettings);
        Toggle(ModConsts.SourceId.ReplacePCCtoSprite, ModContext.Config.Sprite.DefaultTile.Enable, newValue =>
        {
            var oldValue = ModContext.Config.Sprite.DefaultTile.Enable;
            if (oldValue == newValue)
            {
                return;
            }

            ModContext.Config.Sprite.DefaultTile.Enable = newValue;
            if (newValue)
            {
                ModPCRenderer.Initialize();
                ModPCRenderer.Update();
            }
            else
            {
                ModPCRenderer.RevertToPCC();
            }
        });
        Spacer(10);

        Header(ModConsts.SourceId.SpriteSettings);
        AddDefaultTileGroupLayout();
        Spacer(2);
        AddTileGroupLayout(ModContext.Config.Sprite.SnowTile, ModConsts.SourceId.SnowSprite);
        Spacer(2);
        AddTileGroupLayout(ModContext.Config.Sprite.UndressTile, ModConsts.SourceId.UndressSprite);
        Spacer(20);
        AddTileGroupLayout(ModContext.Config.Sprite.RideTile, ModConsts.SourceId.RideSprite);
        Spacer(2);
        AddTileGroupLayout(ModContext.Config.Sprite.RideSnowTile, ModConsts.SourceId.RideSnowSprite);
        Spacer(20);
        AddTileGroupLayout(ModContext.Config.Sprite.CombatTile, ModConsts.SourceId.CombatSprite);
        Spacer(2);

        AddTileGroupLayout(ModContext.Config.Sprite.CombatSnowTile, ModConsts.SourceId.CombatSnowSprite);
        Spacer(20);
        AddTileGroupLayout(ModContext.Config.Sprite.RideCombatTile, ModConsts.SourceId.RideCombatSprite);
        Spacer(2);
        AddTileGroupLayout(ModContext.Config.Sprite.RideCombatSnowTile, ModConsts.SourceId.RideCombatSnowSprite);
    }

    private void AddDefaultTileGroupLayout()
    {
        var tile = ModContext.Config.Sprite.DefaultTile;
        var group = Horizontal().WithFitMode(UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize).WithPivot(0.5f, 0);
        group.Text(ModConsts.SourceId.DefaultSprite, FontColor.Header).WithWidth(250);
        group.InputText(tile.Id.ToString(), HandleOnTileIdInputChange(tile)).WithWidth(100);
    }

    private void AddTileGroupLayout(ModConfigTile tile, string label)
    {
        var group = Horizontal().WithFitMode(UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize).WithPivot(0.5f, 0);
        group.Text(label, FontColor.Header).WithWidth(150);
        group.Toggle(ModConsts.SourceId.Enable, tile.Enable, HandleOnTileEnableToggle(tile)).WithWidth(100);
        group.InputText(tile.Id.ToString(), HandleOnTileIdInputChange(tile)).WithWidth(100);
    }

    private Action<bool> HandleOnTileEnableToggle(ModConfigTile tile)
    {
        return newValue =>
        {
            if (tile.Enable == newValue)
            {
                return;
            }

            tile.Enable = newValue;
            ModPCRenderer.Update();
        };
    }

    private Action<int> HandleOnTileIdInputChange(ModConfigTile tile)
    {
        return newValue =>
        {
            if (tile.Id == newValue)
            {
                return;
            }

            tile.Id = newValue;
            ModPCRenderer.Update();
        };
    }
}

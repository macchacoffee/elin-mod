using System;
using YKF;
using ModUtility.Resource;
using NoPCC.Config;
using NoPCC.Mod;

namespace NoPCC.UI;

public class ModConfigMainTab : YKLayout<object>
{
    public override void OnLayout()
    {
        Header(ModNames.GeneralSettings.Text);
        Toggle(ModNames.EnableMod.Text, ModContext.Config.Sprite.DefaultTile.Enable, newValue =>
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

        Header(ModNames.SpriteSettings.Text);
        AddDefaultTileGroupLayout();
        Spacer(2);
        AddTileGroupLayout(ModContext.Config.Sprite.SnowTile, ModNames.SnowSprite);
        Spacer(2);
        AddTileGroupLayout(ModContext.Config.Sprite.UndressTile, ModNames.UndressSprite);
        Spacer(20);
        AddTileGroupLayout(ModContext.Config.Sprite.RideTile, ModNames.RideSprite);
        Spacer(2);
        AddTileGroupLayout(ModContext.Config.Sprite.RideSnowTile, ModNames.RideSnowSprite);
        Spacer(20);
        AddTileGroupLayout(ModContext.Config.Sprite.CombatTile, ModNames.CombatSprite);
        Spacer(2);

        AddTileGroupLayout(ModContext.Config.Sprite.CombatSnowTile, ModNames.CombatSnowSprite);
        Spacer(20);
        AddTileGroupLayout(ModContext.Config.Sprite.RideCombatTile, ModNames.RideCombatSprite);
        Spacer(2);
        AddTileGroupLayout(ModContext.Config.Sprite.RideCombatSnowTile, ModNames.RideCombatSnowSprite);
    }

    private void AddDefaultTileGroupLayout()
    {
        var tile = ModContext.Config.Sprite.DefaultTile;
        var name = ModNames.DefaultSprite;
        var group = Horizontal().WithFitMode(UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize).WithPivot(0.5f, 0);
        group.Text(name.Text, FontColor.Header).WithWidth(250);
        group.InputText(tile.Id.ToString(), HandleOnTileIdInputChange(tile)).WithWidth(100);
    }

    private void AddTileGroupLayout(ModConfigTile tile, ModName name)
    {
        var group = Horizontal().WithFitMode(UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize).WithPivot(0.5f, 0);
        group.Text(name.Text, FontColor.Header).WithWidth(150);
        group.Toggle(ModNames.Enable.Text, tile.Enable, HandleOnTileEnableToggle(tile)).WithWidth(100);
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

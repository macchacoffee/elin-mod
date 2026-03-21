using System;
using YKF;
using ModUtility.Resource;

namespace NoPCC.UI;

public class ModConfigMainTab : YKLayout<object>
{
    public override void OnLayout()
    {
        Header(ModNames.GeneralSettings.Text);
        Toggle(ModNames.EnableMod.Text, Mod.Config.Sprite.DefaultTile.Enable, newValue =>
        {
            var oldValue = Mod.Config.Sprite.DefaultTile.Enable;
            if (oldValue == newValue)
            {
                return;
            }

            Mod.Config.Sprite.DefaultTile.Enable = newValue;
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
        AddTileGroupLayout(Mod.Config.Sprite.SnowTile, ModNames.SnowSprite);
        Spacer(2);
        AddTileGroupLayout(Mod.Config.Sprite.UndressTile, ModNames.UndressSprite);
        Spacer(20);
        AddTileGroupLayout(Mod.Config.Sprite.RideTile, ModNames.RideSprite);
        Spacer(2);
        AddTileGroupLayout(Mod.Config.Sprite.RideSnowTile, ModNames.RideSnowSprite);
        Spacer(20);
        AddTileGroupLayout(Mod.Config.Sprite.CombatTile, ModNames.CombatSprite);
        Spacer(2);

        AddTileGroupLayout(Mod.Config.Sprite.CombatSnowTile, ModNames.CombatSnowSprite);
        Spacer(20);
        AddTileGroupLayout(Mod.Config.Sprite.RideCombatTile, ModNames.RideCombatSprite);
        Spacer(2);
        AddTileGroupLayout(Mod.Config.Sprite.RideCombatSnowTile, ModNames.RideCombatSnowSprite);
    }

    private void AddDefaultTileGroupLayout()
    {
        var tile = Mod.Config.Sprite.DefaultTile;
        var name = ModNames.DefaultSprite;
        var group = Horizontal().WithFitMode(UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize).WithPivot(0.5f, 0);
        group.Text(name.Text, FontColor.Header).WithWidth(250);
        group.InputText(tile.Id.ToString(), HandleOnTileIdInputChange(tile)).WithWidth(100);
    }

    private void AddTileGroupLayout(Tile tile, ModName name)
    {
        var group = Horizontal().WithFitMode(UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize).WithPivot(0.5f, 0);
        group.Text(name.Text, FontColor.Header).WithWidth(150);
        group.Toggle(ModNames.Enable.Text, tile.Enable, HandleOnTileEnableToggle(tile)).WithWidth(100);
        group.InputText(tile.Id.ToString(), HandleOnTileIdInputChange(tile)).WithWidth(100);
    }

    private Action<bool> HandleOnTileEnableToggle(Tile tile)
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

    private Action<int> HandleOnTileIdInputChange(Tile tile)
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

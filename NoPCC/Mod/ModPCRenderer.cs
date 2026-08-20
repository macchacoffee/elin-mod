using Macchacoffee.ElinMods.NoPCC.Config;

namespace Macchacoffee.ElinMods.NoPCC.Mod;

internal static class ModPCRenderer
{
    private static CharaRenderer? modRenderer;

    public static void Initialize()
    {
        modRenderer = null;
    }

    public static void RevertToPCC()
    {
        EClass.pc._CreateRenderer();
        modRenderer = null;
    }

    public static void Update()
    {
        if (!ModContext.Config.Sprite.DefaultTile.Enable || EClass.game.isLoading)
        {
            return;
        }

        var pc = EClass.pc;
        var prevRenderer = pc.renderer;

        var isTrunsmuted = false;
        if (modRenderer is null || modRenderer != prevRenderer)
        {
            // Initialize呼び出し後に初めてUpdateが呼び出された、またはpcのRendererがModのものと異なる場合、
            // pcのRendererが存在すれば変容中とみなす
            isTrunsmuted = prevRenderer.replacer is not null;
        }

        if (modRenderer is null)
        {
            var rendererReplacer = RendererReplacer.CreateFrom("adv", 0);
            modRenderer = new CharaRenderer
            {
                replacer = rendererReplacer,
                data = rendererReplacer.data
            };
        }

        if (isTrunsmuted)
        {
            return;
        }

        var tile = SelectTile(ModContext.Config.Sprite.DefaultTile, ModContext.Config.Sprite.SnowTile, null);
        if (pc.pccData.state == PCCState.Naked || pc.pccData.state == PCCState.Undie)
        {
            tile = SelectTile(ModContext.Config.Sprite.UndressTile, tile);
        }
        else if (pc.ride is not null)
        {
            if (pc.combatCount > 0)
            {
                tile = SelectTile(
                    ModContext.Config.Sprite.RideCombatTile,
                    ModContext.Config.Sprite.RideCombatSnowTile,
                    tile);
            }
            else
            {
                tile = SelectTile(ModContext.Config.Sprite.RideTile, ModContext.Config.Sprite.RideSnowTile, tile);
            }
        }
        else if (pc.combatCount > 0)
        {
            tile = SelectTile(ModContext.Config.Sprite.CombatTile, ModContext.Config.Sprite.CombatSnowTile, tile);
        }

        if (tile is null || tile.Id < 0 || (modRenderer == prevRenderer && modRenderer.replacer.tile == tile.Id))
        {
            return;
        }

        modRenderer.replacer.tile = tile.Id / 100 * 32 + tile.Id % 100;
        pc.renderer = modRenderer;
        pc.renderer.SetOwner(pc);
    }

    private static ModConfigTile? SelectTile(ModConfigTile tile, ModConfigTile? defaultTile)
    {
        return SelectTile(tile, null, defaultTile);
    }

    private static ModConfigTile? SelectTile(ModConfigTile tile, ModConfigTile? snowTile, ModConfigTile? defaultTile)
    {
        var selected = defaultTile;
        if (snowTile is not null && snowTile.Enable && EClass._zone.IsSnowCovered)
        {
            selected = snowTile;
        }
        else if (tile.Enable)
        {
            selected = tile;
        }

        return selected;
    }
}

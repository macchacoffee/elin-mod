namespace NoPCC;

public static class ModPCRenderer
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
        if (!Mod.Config.Sprite.DefaultTile.Enable || EClass.game.isLoading)
        {
            return;
        }

        var pc = EClass.pc;
        var prevRenderer = pc.renderer;

        var isTrunsmuted = false;
        if (modRenderer == null)
        {
            // On first update after initializing.
            isTrunsmuted = prevRenderer.replacer != null;
        }
        else if (modRenderer != prevRenderer)
        {
            // On update with transmuting.
            isTrunsmuted = prevRenderer.replacer != null;
        }

        if (modRenderer == null)
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

        var tile = SelectTile(Mod.Config.Sprite.DefaultTile, Mod.Config.Sprite.SnowTile, null);
        if (pc.pccData.state == PCCState.Naked || pc.pccData.state == PCCState.Undie)
        {
            tile = SelectTile(Mod.Config.Sprite.UndressTile, tile);
        }
        else if (pc.ride != null)
        {
            if (pc.combatCount > 0)
            {
                tile = SelectTile(Mod.Config.Sprite.RideCombatTile, Mod.Config.Sprite.RideCombatSnowTile, tile);
            }
            else
            {
                tile = SelectTile(Mod.Config.Sprite.RideTile, Mod.Config.Sprite.RideSnowTile, tile);
            }
        }
        else if (pc.combatCount > 0)
        {
            tile = SelectTile(Mod.Config.Sprite.CombatTile, Mod.Config.Sprite.CombatSnowTile, tile);
        }

        if (tile == null || tile.Id < 0 || (modRenderer == prevRenderer && modRenderer.replacer.tile == tile.Id))
        {
            return;
        }

        modRenderer.replacer.tile = tile.Id / 100 * 32 + tile.Id % 100;
        pc.renderer = modRenderer;
        pc.renderer.SetOwner(pc);
    }

    private static Tile? SelectTile(Tile tile, Tile? defaultTile)
    {
        return SelectTile(tile, null, defaultTile);
    }

    private static Tile? SelectTile(Tile tile, Tile? snowTile, Tile? defaultTile)
    {
        var selected = defaultTile;
        if (snowTile != null && snowTile.Enable && EClass._zone.IsSnowCovered)
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

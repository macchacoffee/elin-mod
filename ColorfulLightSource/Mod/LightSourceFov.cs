namespace Macchacoffee.ElinMods.ColorfulLightSource.Mod;

internal static class LightSourceFov
{
    public static void UpdateEquippedLightFov(Card card, Fov fov)
    {
        if (!card.isChara || !card.IsPCFaction)
        {
            return;
        }

        var light = card.Chara.body.GetEquippedThing(SLOT.lightsource);
        if (light?.trait is not TraitLightSource)
        {
            return;
        }

        var lightFov = light.CreateFov();

        fov.r = lightFov.r;
        fov.g = lightFov.g;
        fov.b = lightFov.b;
    }

    public static void RecalculateEquippedLightFovs()
    
    {
        if (EClass._map == null || EClass._zone == null || !EClass._zone.isStarted)
        {
            return;
        }

        foreach (var chara in EClass._map.charas)
        {
            if (chara.IsPC || !chara.IsPCFaction)
            {
                continue;
            }

            var light = chara.body.GetEquippedThing(SLOT.lightsource);
            if (light?.trait is TraitLightSource)
            {
                chara.RecalculateFOV();
            }
        }
    }
}

using System;
using System.Linq;
using SomewhatEnhancedDisplay.Config;
using SomewhatEnhancedDisplay.Extensions;
using UnityEngine;

namespace SomewhatEnhancedDisplay.UI.HoverGuide;

public static class ModThingHoverTextBuilder
{
    private static ModConfigHoverGuide Config => Mod.Config.HoverGuide;
    private static ModConfigHoverGuideColorSet ColorConfig => Config.ColorSet;
    private static ModConfigHoverGuideStyleThing StyleConfig => Config.CurrentStyle.Thing;

    public static string BuildHoverText(Thing thing, string cardText, string text)
    {
        // cardText: Card.GetHoverText()の戻り値 (名前)
        // text: thingの追加情報
        cardText = StyleConfig.UseRarityColor ? cardText.TagColorIfNotEmptyNullable(GetRarityColor(thing)) : cardText; 
        return string.Join(" ", new[] {
            GetHoverTextLv(thing)?.TagSize(ModUIUtil.ComputeFontSize(11)),
            $"{cardText}{text}"
        }.Where(t => !string.IsNullOrEmpty(t)));
    }

    public static string BuildHoverText2(Thing thing, string text, string traitText)
    {
        // text: thing.GetHoverText2()の戻り値 (空文字列)
        // traitText: trait.GetHoverText()の戻り値
        traitText = !string.IsNullOrEmpty(traitText) ? traitText.TagSize(ModUIUtil.ComputeFontSize(11)) : traitText;
        return string.Join(Environment.NewLine, new[] {
            text.TagSizeIfNotEmpty(ModUIUtil.ComputeFontSize(11)),
            GetHoverTextExtra1(thing)?.TagSize(ModUIUtil.ComputeFontSize(11)).TagColorNullable(ColorConfig.SubTextColor),
            traitText
        }.Where(t => !string.IsNullOrEmpty(t)));
    }

    private static string? GetHoverTextLv(Thing thing)
    {
        return StyleConfig.DisplayLv ? GetLvText(thing) : null;
    }

    private static string? GetHoverTextExtra1(Thing thing)
    {
        var text = string.Join(" ", new[] {
            StyleConfig.DisplayMaterial ? GetMaterialText(thing) : null,
            StyleConfig.DisplayFressness ? GetFressnessText(thing) : null,
            StyleConfig.DisplayLockLv ? GetLockLvText(thing) : null,
        }.Where(t => !string.IsNullOrEmpty(t)));
        return !string.IsNullOrEmpty(text) ? text : null;
    }

    private static string GetLvText(Thing thing)
    {
        return $"Lv.{thing.LV}";
    }

    private static string? GetMaterialText(Thing thing)
    {
        var material = thing.material;
        return material.GetName().TagColorNullable(GetMaterialColor(material.alias));
    }

    private static string? GetFressnessText(Thing thing)
    {
        if (thing.trait.Decay == 0)
        {
            // 腐らない場合
            return null;
        }
        var ratio = (float)(thing.MaxDecay - Math.Min(thing.decay, thing.MaxDecay)) / thing.MaxDecay;
        var pct = Math.Ceiling(ratio * 100);
        return $"{ModConsts.SourceId.Fressness.lang()}:{pct:0}%".TagColor(Color.Lerp(ColorConfig.FressnessLowValueColor, ColorConfig.FressnessValueColor, ratio));
    }

    private static string? GetLockLvText(Thing thing)
    {
        if (thing.c_lockLv == 0)
        {
            // 施錠されていない場合
            return null;
        }
        return $"{ModConsts.SourceId.LockLv.lang()}:{thing.c_lockLv}";
    }

    private static Color? GetMaterialColor(string alias)
    {
        if (EClass.Colors.matColors.TryGetValue(alias, out var matColor))
        {
            return Color.Lerp(matColor.main, Color.white, 0.4f);
        }
        return null;
    }

    private static Color? GetRarityColor(Thing thing)
    {
        return thing.rarity switch
        {
            Rarity.Crude => ColorConfig.RarityCrudeColor,
            Rarity.Normal => ColorConfig.RarityNormalColor,
            Rarity.Superior => ColorConfig.RaritySuperiorColor,
            Rarity.Legendary => ColorConfig.RarityLegendaryColor,
            Rarity.Mythical => ColorConfig.RarityMythicalColor,
            Rarity.Artifact => ColorConfig.RarityArtifactColor,
            _ => null,
        };
    }
}

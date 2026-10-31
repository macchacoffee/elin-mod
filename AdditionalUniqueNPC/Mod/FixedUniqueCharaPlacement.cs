using System;
using System.Collections.Generic;

using Macchacoffee.ElinMods.ModUtility.Logging;

namespace Macchacoffee.ElinMods.AdditionalUniqueNPC.Mod;

internal static class FixedUniqueCharaPlacement
{
    private sealed record Placement(string ZoneFullName, int X, int Z);

    private static readonly Dictionary<string, Placement> Placements = new(StringComparer.Ordinal)
    {
        { ModConsts.SourceId.Misalie, new("tinkerCamp", 69, 42) },
        { ModConsts.SourceId.Eirwen, new("palmia", 60, 70) },
    };

    [ElinPostLoad]
    private static void OnPostLoad(GameIOContext context)
    {
        foreach (var entry in Placements)
        {
            try
            {
                Initialize(entry.Key, entry.Value);
            }
            catch (Exception ex)
            {
                ModLog.Error($"Failed to initialize fixed unique character '{entry.Key}': {ex}");
            }
        }
    }

    public static void EnsureReturnLocation(Chara chara)
    {
        if (!chara.IsGlobal || !Placements.TryGetValue(chara.id, out var placement))
        {
            return;
        }

        EnsureReturnLocation(chara, placement);
    }

    private static void Initialize(string id, Placement placement)
    {
        var chara = EClass.game.cards.globalCharas.Find(id);
        if (chara != null)
        {
            EnsureReturnLocation(chara, placement);
            return;
        }

        if (!EClass.sources.charas.map.ContainsKey(id))
        {
            ModLog.Error($"Cannot place fixed unique character '{id}': source row not found.");
            return;
        }

        var zone = FindZone(id, placement);
        if (zone == null)
        {
            return;
        }

        chara = CharaGen.Create(id).SetGlobal(zone, placement.X, placement.Z);
        // 配置先のゾーンが現在処理中のゾーンである場合、配置はすでに完了しているため、
        // 再度ゾーンが有効化された時に不要な再配置が行われないようにする。
        if (zone.IsActiveZone)
        {
            chara.global.transition = null;
        }

        SetReturnLocation(chara, zone, placement);
    }

    private static void EnsureReturnLocation(Chara chara, Placement placement)
    {
        var uniqueData = chara.c_uniqueData;
        if (uniqueData != null &&
            EClass.game.spatials.map.TryGetValue(uniqueData.uidZone, out var spatial) &&
            spatial is Zone)
        {
            return;
        }

        var zone = FindZone(chara.id, placement);
        if (zone != null)
        {
            // 生成済みのキャラクターに追放後の帰還先がない場合は設定する。
            SetReturnLocation(chara, zone, placement);
        }
    }

    private static Zone? FindZone(string id, Placement placement)
    {
        var zone = ModUtil.FindZoneByFullName(placement.ZoneFullName);
        if (zone == null)
        {
            ModLog.Error($"Cannot place fixed unique character '{id}': zone '{placement.ZoneFullName}' not found.");
        }

        return zone;
    }

    private static void SetReturnLocation(Chara chara, Zone zone, Placement placement)
    {
        chara.c_uniqueData = new()
        {
            uidZone = zone.uid,
            x = placement.X,
            y = placement.Z
        };
    }
}

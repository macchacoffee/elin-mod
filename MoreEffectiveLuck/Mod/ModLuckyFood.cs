using System;
using System.Collections.Generic;

namespace MoreEffectiveLuck.Mod;

internal class ModLuckyFood
{
    private static readonly Dictionary<string, ModLuckyFood> IdToFood = new()
    {
        ["kagamimochi"] = new(power: 10),
        ["churyu"] = new(power: 22),
        ["wedding_cake1"] = new(power: 20),
        ["bushdenoel"] = new(power: 10),
        ["crimale2"] = new(power: 11),
        ["65_gold"] = new(power: 77),   // 金のコイ
        ["86"] = new(power: 5),         // マダイ
        ["71"] = new(power: 5),         // シロアマダイ
        ["_poop"] = new(getPower: f => f.material.alias switch
        {
            "gold" => 77,
            "silver" => 7,
            _ => 0
        })
    };

    private Func<Thing, int> GetPower { get; }

    private ModLuckyFood(int power) : this(_ => power) { }

    private ModLuckyFood(Func<Thing, int> getPower)
    {
        GetPower = getPower;
    }

    public static bool IsLuckyFood(Thing thing)
    {
        return GetLuckyPower(thing) > 0;
    }

    public static void ProcFoodEffect(Chara chara, Thing thing)
    {
        var power = GetLuckyPower(thing);
        if (thing.IsBlessed)
        {
            var rnd = EClass.rnd(100);
            if (rnd == 0)
            {
                 power += 50;
            }
            else if (rnd < 0 && rnd <= 5)
            {
                power += 20;
            }
            else if (rnd < 5 && rnd <= 15)
            {
                power += 10;
            }
        }
        if (power > 0)
        {
            chara.AddCondition<ConMCMELLucky>(power);
        }
    }

    private static int GetLuckyPower(Thing thing)
    {
        return IdToFood.TryGetValue(thing.source.id, out var luckyFood) ? luckyFood.GetPower(thing) : 0;
    }
}

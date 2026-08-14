using ModUtility.Config;
using Newtonsoft.Json;
using UnityEngine;

namespace SimpleDamageTracker.Config;

internal enum ModHorizontalTextAlignment
{
    Left = 0,
    Center,
    Right,
}

internal class ModWorldConfig : JsonModConfigBase<ModWorldConfig>
{
    [JsonProperty("display", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigDisplay Display { get; set; } = new();

    public void ResetDisplay()
    {
        Display = new();
    }
}

internal class ModConfigDisplay : JsonModConfigBase<ModConfigDisplay>
{
    [JsonProperty("displayNoDamage", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayNoDamage { get; set; } = false;

    [JsonProperty("damage", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigDisplayText Damage { get; set; } = new()
    {
        Display = true,
        X = 0f,
        Y = -15f,
        SizeScale = 1f,
        HorizontalAlignment = ModHorizontalTextAlignment.Left,
        Color = new(1f, 1f, 1f), // #FFFFFFFF
    };

    [JsonProperty("DamageShare", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigDisplayText DamageShare { get; set; } = new()
    {
        Display = true,
        X = 55f,
        Y = -15f,
        SizeScale = 0.9f,
        HorizontalAlignment = ModHorizontalTextAlignment.Left,
        Color = new(1f, 1f, 1f), // #FFFFFFFF
    };
}

internal class ModConfigDisplayText : JsonModConfigBase<ModConfigDisplayText>
{
    [JsonProperty("display", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool Display { get; set; }

    [JsonProperty("x", DefaultValueHandling = DefaultValueHandling.Include)]
    public float X { get; set; }

    [JsonProperty("y", DefaultValueHandling = DefaultValueHandling.Include)]
    public float Y { get; set; }

    [JsonProperty("sizeScale", DefaultValueHandling = DefaultValueHandling.Include)]
    public float SizeScale { get; set; }

    [JsonProperty("horizontalAlignment", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModHorizontalTextAlignment HorizontalAlignment { get; set; }

    [JsonProperty("color", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ModColorConverter))]
    public Color Color { get; set; }
}

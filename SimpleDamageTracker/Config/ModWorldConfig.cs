using ModUtility.Config;
using Newtonsoft.Json;

namespace SimpleDamageTracker.Config;

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
        X = 0f,
        Y = -15f,
        Size = 0,
    };

    [JsonProperty("percentage", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigDisplayText Percentage { get; set; } = new()
    {
        X = 55f,
        Y = -15f,
        Size = -2,
    };
}

internal class ModConfigDisplayText : JsonModConfigBase<ModConfigDisplayText>
{
    [JsonProperty("x", DefaultValueHandling = DefaultValueHandling.Include)]
    public float X { get; set; }

    [JsonProperty("y", DefaultValueHandling = DefaultValueHandling.Include)]
    public float Y { get; set; }

    [JsonProperty("size", DefaultValueHandling = DefaultValueHandling.Include)]
    public int Size { get; set; }
}

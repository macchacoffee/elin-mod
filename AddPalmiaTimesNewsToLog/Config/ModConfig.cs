using ModUtility.Config;
using Newtonsoft.Json;
using UnityEngine;

namespace AddPalmiaTimesNewsToLog.Config;

public enum ModLogTarget
{
    Log = 1,
    Feed
}

public class ModConfig : ModConfigBase<ModConfig>
{
    [JsonProperty("enable", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool Enable { get; set; } = true;

    [JsonProperty("frequencyMinute", DefaultValueHandling = DefaultValueHandling.Include)]
    public int FrequencyMinute { get; set; } = 10;

    [JsonProperty("logTarget", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModLogTarget LogTarget { get; set; } = ModLogTarget.Log;

    [JsonProperty("logColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ModColorConverter))]
    public Color LogColor { get; set; } = new(0.403f, 0.381f, 0.336f); // #B3AEA2FF

    [JsonProperty("news", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigNews News { get; private set; } = new();

    [JsonProperty("chat", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigChat Chat { get; private set; } = new();
}

public class ModConfigNews : ModConfigBase<ModConfig>
{
    [JsonProperty("enable", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool Enable { get; set; } = true;

    [JsonProperty("maxCount", DefaultValueHandling = DefaultValueHandling.Include)]
    public int MaxCount { get; set; } = 1;
}

public class ModConfigChat : ModConfigBase<ModConfig>
{
    [JsonProperty("enable", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool Enable { get; set; } = true;

    [JsonProperty("maxCount", DefaultValueHandling = DefaultValueHandling.Include)]
    public int MaxCount { get; set; } = 2;

    [JsonProperty("fetchDead", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool FetchDead { get; set; } = true;

    [JsonProperty("fetchWish", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool FetchWish { get; set; } = true;

    [JsonProperty("fetchMarriage", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool FetchMarriage { get; set; } = true;
}

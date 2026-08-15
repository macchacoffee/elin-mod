using ModUtility.Config;
using Newtonsoft.Json;

namespace NoPCC.Config;

internal class ModConfig : JsonModConfigBase<ModConfig>
{
    [JsonProperty("sprite", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigSprite Sprite { get; private set; } = new();
}

internal class ModConfigSprite : JsonModConfigBase<ModConfigSprite>
{
    // お兄ちゃん お兄ちゃん！ お兄ちゃん？ お兄ちゃん！！
    private const int _initialTileId = 1918;
    private const int _initialSnowTileId = 1919;
    private const int _initialEmptyTileId = 0;

    [JsonProperty("defaultTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigTile DefaultTile { get; private set; } = new() { Enable = true, Id = _initialTileId };

    [JsonProperty("snowTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigTile SnowTile { get; private set; } =  new() { Enable = false, Id = _initialSnowTileId };

    [JsonProperty("undressTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigTile UndressTile { get; private set;} = new() { Enable = false, Id = _initialEmptyTileId };

    [JsonProperty("rideTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigTile RideTile { get; private set; } = new() { Enable = false, Id = _initialEmptyTileId };

    [JsonProperty("rideSnowTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigTile RideSnowTile { get; private set; } = new() { Enable = false, Id = _initialEmptyTileId };

    [JsonProperty("combatTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigTile CombatTile { get; private set; } = new() { Enable = false, Id = _initialEmptyTileId };

    [JsonProperty("combatSnowTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigTile CombatSnowTile { get; private set; } = new() { Enable = false, Id = _initialEmptyTileId };

    [JsonProperty("rideCombatTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigTile RideCombatTile { get; private set; } = new() { Enable = false, Id = _initialEmptyTileId };

    [JsonProperty("rideCombatSnowTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigTile RideCombatSnowTile { get; private set; } = new() { Enable = false, Id = _initialEmptyTileId };
}

internal class ModConfigTile : JsonModConfigBase<ModConfigTile>
{
    [JsonProperty("enable", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool Enable { get; set; }

    [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Include)]
    public int Id { get; set; }
}

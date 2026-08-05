using ModUtility.Config;
using Newtonsoft.Json;

namespace NoPCC.Config;

public class ModConfig : JsonModConfigBase<ModConfig>
{
    [JsonProperty("sprite", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigSprite Sprite { get; private set; } = new();
}

public class ModConfigSprite : JsonModConfigBase<ModConfigSprite>
{
    // お兄ちゃん お兄ちゃん！ お兄ちゃん？ お兄ちゃん！！
    private static readonly int InitialTileId = 1918;
    private static readonly int InitialSnowTileId = 1919;
    private static readonly int InitialEmptyTileId = 0;

    [JsonProperty("defaultTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigTile DefaultTile { get; private set; } = new() { Enable = true, Id = InitialTileId };

    [JsonProperty("snowTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigTile SnowTile { get; private set; } =  new() { Enable = false, Id = InitialSnowTileId };

    [JsonProperty("undressTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigTile UndressTile { get; private set;} = new() { Enable = false, Id = InitialEmptyTileId };

    [JsonProperty("rideTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigTile RideTile { get; private set; } = new() { Enable = false, Id = InitialEmptyTileId };

    [JsonProperty("rideSnowTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigTile RideSnowTile { get; private set; } = new() { Enable = false, Id = InitialEmptyTileId };

    [JsonProperty("combatTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigTile CombatTile { get; private set; } = new() { Enable = false, Id = InitialEmptyTileId };

    [JsonProperty("combatSnowTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigTile CombatSnowTile { get; private set; } = new() { Enable = false, Id = InitialEmptyTileId };

    [JsonProperty("rideCombatTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigTile RideCombatTile { get; private set; } = new() { Enable = false, Id = InitialEmptyTileId };

    [JsonProperty("rideCombatSnowTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigTile RideCombatSnowTile { get; private set; } = new() { Enable = false, Id = InitialEmptyTileId };
}

public class ModConfigTile : JsonModConfigBase<ModConfigTile>
{
    [JsonProperty("enable", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool Enable { get; set; }

    [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Include)]
    public int Id { get; set; }
}

using Newtonsoft.Json;

namespace NoPCC;

public class ModConfig
{
    [JsonProperty("sprite", DefaultValueHandling = DefaultValueHandling.Include)]
    public Sprite Sprite { get; init; } = new();
}

public class Sprite
{
    // お兄ちゃん お兄ちゃん！ お兄ちゃん？ お兄ちゃん！！
    private static readonly int InitialTileId = 1918;
    private static readonly int InitialSnowTileId = 1919;
    private static readonly int InitialEmptyTileId = 0;

    [JsonProperty("defaultTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public Tile DefaultTile { get; } = new(true, InitialTileId);
    [JsonProperty("snowTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public Tile SnowTile { get; } = new(false, InitialSnowTileId);
    [JsonProperty("undressTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public Tile UndressTile { get; } = new(false, InitialEmptyTileId);
    [JsonProperty("rideTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public Tile RideTile { get; } = new(false, InitialEmptyTileId);
    [JsonProperty("rideSnowTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public Tile RideSnowTile { get; } = new(false, InitialEmptyTileId);
    [JsonProperty("combatTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public Tile CombatTile { get; } = new(false, InitialEmptyTileId);
    [JsonProperty("combatSnowTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public Tile CombatSnowTile { get; } = new(false, InitialEmptyTileId);
    [JsonProperty("rideCombatTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public Tile RideCombatTile { get; } = new(false, InitialEmptyTileId);
    [JsonProperty("rideCombatSnowTile", DefaultValueHandling = DefaultValueHandling.Include)]
    public Tile RideCombatSnowTile { get; } = new(false, InitialEmptyTileId);
}

public record Tile
{
    [JsonProperty("enable", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool Enable { get; set; }
    [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Include)]
    public int Id { get; set; }

    public Tile(bool enable, int id)
    {
        Enable = enable;
        Id = id;
    }
}

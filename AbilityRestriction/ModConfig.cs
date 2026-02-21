using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AbilityRestriction;

[JsonConverter(typeof(ModConfigConverter))]
public class ModConfig
{
    [JsonProperty("deniedAbilities")]
    public ModDeniedAbilities DeniedAbilities { get; init; } = new ModDeniedAbilities();

    public ModDeniedAbility? GetDeniedAbility(int uid)
    {
        return DeniedAbilities.TryGetValue(uid, out var ability) ? ability : null;
    }

    public void SetDeniedAbility(int uid, ModDeniedAbility deniedAbility)
    {
        DeniedAbilities[uid] = deniedAbility;
    }

    public bool RemoveDeniedAbility(int uid)
    {
        return DeniedAbilities.Remove(uid);
    }

    public void CleanUp()
    {
        foreach (var pair in DeniedAbilities.ToArray())
        {
            var uid = pair.Key;
            var ability = pair.Value;
            if (ability.IsEmpty() || !EClass.game.cards.globalCharas.ContainsKey(uid))
            {
                // Remove if denied ability is empty or its owner has gone from game.
                DeniedAbilities.Remove(uid);
            }
        }
    }
}

public class ModDeniedAbilities : Dictionary<int, ModDeniedAbility>;

public class ModDeniedAbility
{
    [JsonProperty("acts")]
    [JsonConverter(typeof(ModDeniedActConverter))]
    public HashSet<ModDeniedAct> Acts { get; init; } = new HashSet<ModDeniedAct>();

    public bool Contains(ModDeniedAct act)
    {
        return Acts.Contains(act);
    }

    public bool IsEmpty()
    {
        return Acts.Count() == 0;
    }

    public int Count()
    {
        return Acts.Count();
    }

    public bool Add(ModDeniedAct act)
    {
        return Acts.Add(act);
    }

    public bool Remove(ModDeniedAct act)
    {
        return Acts.Remove(act);
    }

    public void IntersectWith(IEnumerable<ModDeniedAct> otherActs)
    {
        Acts.IntersectWith(otherActs);
    }
}

public record ModDeniedAct
{
    [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Include)]
    public int Id { get; init; }
    [JsonProperty("pt", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool Pt { get; init; }

    public ModDeniedAct(int id, bool pt)
    {
        Id = id;
        Pt = pt;
    }

    public ModDeniedAct(ActList.Item act) : this(act.act.id, act.pt) { }
}

// Converters for migrating old config data to new one.
public class ModConfigConverter : JsonConverter<ModConfig>
{
    public override bool CanWrite => false;

    public override ModConfig ReadJson(JsonReader reader, Type objectType, ModConfig existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);
        if (token is not JObject obj)
        {
            throw new JsonSerializationException($"Unexpected JSON format in ModConfigConverter: {token}");
        }

        if (obj.ContainsKey("deniedAbilities"))
        {
            var config = new ModConfig();
            serializer.Populate(obj.CreateReader(), config);
            return config;
        }
        else
        {
            var deniedAbilities = obj.ToObject<ModDeniedAbilities>();
            return new ModConfig
            {
                DeniedAbilities = deniedAbilities
            };
        }
    }

    public override void WriteJson(JsonWriter writer, ModConfig value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}

public class ModDeniedActConverter : JsonConverter<HashSet<ModDeniedAct>>
{
    public override bool CanWrite => false;

    public override HashSet<ModDeniedAct> ReadJson(JsonReader reader, Type objectType, HashSet<ModDeniedAct> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);
        if (token is not JArray array)
        {
            throw new JsonSerializationException($"Unexpected JSON format in ModDeniedActConverter: {token}");
        }

        var acts = new HashSet<ModDeniedAct>();
        foreach (var element in array)
        {
            if (element.Type == JTokenType.Integer)
            {
                var id = element.ToObject<int>();
                acts.Add(new ModDeniedAct(id, false));
                acts.Add(new ModDeniedAct(id, true));
            }
            else
            {
                var id = element["id"].ToObject<int>();
                bool pt = element["pt"].ToObject<bool>();
                acts.Add(new ModDeniedAct(id, pt));
            }
        }

        return acts;
    }

    public override void WriteJson(JsonWriter writer, HashSet<ModDeniedAct> value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}

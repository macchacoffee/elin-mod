using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Macchacoffee.ElinMods.ModUtility.Config;

namespace Macchacoffee.ElinMods.AbilityRestriction.Config;

[JsonConverter(typeof(ModConfigConverter))]
internal class ModWorldConfig : JsonModConfigBase<ModWorldConfig>
{
    [JsonProperty("deniedAbilities")]
    private ModConfigDeniedAbilities DeniedAbilities { get; set; } = [];

    public ModWorldConfig() { }

    public ModWorldConfig(ModConfigDeniedAbilities deniedAbilities)
    {
        DeniedAbilities = deniedAbilities;
    }

    public ModConfigDeniedAbility? GetDeniedAbility(int uid)
    {
        return DeniedAbilities.TryGetValue(uid, out var ability) ? ability : null;
    }

    public bool AddDeniedAct(int uid, ModConfigDeniedAct act)
    {
        if (!DeniedAbilities.TryGetValue(uid, out var deniedAbility))
        {
            deniedAbility = new();
            DeniedAbilities.Add(uid, deniedAbility);
        }

        return deniedAbility.Add(act);
    }

    public bool RemoveDeniedAct(int uid, ModConfigDeniedAct act)
    {
        if (!DeniedAbilities.TryGetValue(uid, out var deniedAbility))
        {
            return false;
        }

        var removed = deniedAbility.Remove(act);
        if (deniedAbility.IsEmpty)
        {
            DeniedAbilities.Remove(uid);
        }
        return removed;
    }
}

internal class ModConfigDeniedAbilities : Dictionary<int, ModConfigDeniedAbility>;

internal class ModConfigDeniedAbility
{
    [JsonProperty("acts")]
    [JsonConverter(typeof(ModConfigDeniedActConverter))]
    private HashSet<ModConfigDeniedAct> Acts { get; set; } = [];

    [JsonIgnore]
    public bool IsEmpty => Acts.Count == 0;

    [JsonIgnore]
    public int Count => Acts.Count;

    public bool Contains(ModConfigDeniedAct act)
    {
        return Acts.Contains(act);
    }

    public bool ContainsId(int actId)
    {
        return Acts.Any(a => a.Id == actId);
    }

    public bool Add(ModConfigDeniedAct act)
    {
        return Acts.Add(act);
    }

    public bool Remove(ModConfigDeniedAct act)
    {
        return Acts.Remove(act);
    }
}

internal record ModConfigDeniedAct
{
    [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Include)]
    public int Id { get; init; }
    [JsonProperty("pt", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool Pt { get; init; }

    public ModConfigDeniedAct(int id, bool pt)
    {
        Id = id;
        Pt = pt;
    }

    public ModConfigDeniedAct(ActList.Item act) : this(act.act.id, act.pt) { }
}

// 旧形式の設定を新形式にマイグレーションするコンバータ
internal class ModConfigConverter : JsonConverter<ModWorldConfig>
{
    public override bool CanWrite => false;

    public override ModWorldConfig ReadJson(
        JsonReader reader,
        Type objectType,
        ModWorldConfig existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        var token = JToken.Load(reader);
        if (token is not JObject obj)
        {
            throw new JsonSerializationException($"Unexpected JSON format in ModConfigConverter: {token}");
        }

        if (obj.ContainsKey("deniedAbilities"))
        {
            var config = new ModWorldConfig();
            serializer.Populate(obj.CreateReader(), config);
            return config;
        }
        else
        {
            var deniedAbilities = obj.ToObject<ModConfigDeniedAbilities>();
            return new(deniedAbilities ?? []);
        }
    }

    public override void WriteJson(JsonWriter writer, ModWorldConfig value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}

internal class ModConfigDeniedActConverter : JsonConverter<HashSet<ModConfigDeniedAct>>
{
    public override bool CanWrite => false;

    public override HashSet<ModConfigDeniedAct> ReadJson(
        JsonReader reader,
        Type objectType,
        HashSet<ModConfigDeniedAct> existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        var token = JToken.Load(reader);
        if (token is not JArray array)
        {
            throw new JsonSerializationException($"Unexpected JSON format in ModDeniedActConverter: {token}");
        }

        var acts = new HashSet<ModConfigDeniedAct>();
        foreach (var element in array)
        {
            if (element.Type == JTokenType.Integer)
            {
                var id = element.ToObject<int>();
                acts.Add(new(id, false));
                acts.Add(new(id, true));
            }
            else
            {
                var id = element["id"].ToObject<int>();
                bool pt = element["pt"].ToObject<bool>();
                acts.Add(new(id, pt));
            }
        }

        return acts;
    }

    public override void WriteJson(JsonWriter writer, HashSet<ModConfigDeniedAct> value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}

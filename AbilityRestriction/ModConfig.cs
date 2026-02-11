using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace AbilityRestriction;

public class ModConfig
{
    [JsonProperty("DeniedAbilities")]
    private ModDeniedAbilities deniedAbilities = new ModDeniedAbilities();

    public ModDeniedAbility? GetDeniedAbility(int uid)
    {
        return deniedAbilities.TryGetValue(uid, out var ability) ? ability : null;
    }

    public void SetDeniedAbility(int uid, ModDeniedAbility deniedAbility)
    {
        deniedAbilities[uid] = deniedAbility;
    }

    public bool RemoveDeniedAbility(int uid)
    {
        return deniedAbilities.Remove(uid);
    }

    public void Load(string filePath)
    {
        if (File.Exists(filePath))
        {
            var text = IO.IsCompressed(filePath) ? IO.Decompress(filePath) : File.ReadAllText(filePath);
            deniedAbilities = JsonConvert.DeserializeObject<ModDeniedAbilities>(text, GameIO.jsReadGame);
        }
        else
        {
            deniedAbilities = new ModDeniedAbilities();
        }

        cleanUp();
    }

    public void Save(string filePath)
    {
        cleanUp();

        var text = JsonConvert.SerializeObject(deniedAbilities, GameIO.formatting, GameIO.jsWriteGame);
        if (GameIO.compressSave)
        {
            IO.Compress(filePath, text);
        }
        else
        {
            File.WriteAllText(filePath, text);
        }
    }

    private void cleanUp()
    {
        foreach (var uid in deniedAbilities.Keys.ToArray())
        {
            if (EClass.game.cards.globalCharas.ContainsKey(uid))
            {
                continue;
            }
            deniedAbilities.Remove(uid);
        }
    }
}

public class ModDeniedAbilities : Dictionary<int, ModDeniedAbility> { }

public class ModDeniedAbility
{
    [JsonProperty("acts")]
    private readonly HashSet<int> acts;

    public ModDeniedAbility() : this([]) { }

    public ModDeniedAbility(int[] acts)
    {
        this.acts = [.. acts];
    }

    public bool Contains(int act)
    {
        return acts.Contains(act);
    }

    public bool IsEmpty()
    {
        return acts.Count() == 0;
    }

    public int Count()
    {
        return acts.Count();
    }

    public bool Add(int act)
    {
        return acts.Add(act);
    }

    public bool Remove(int act)
    {
        return acts.Remove(act);
    }

    public void IntersectWith(IEnumerable<int> otherActs)
    {
        acts.IntersectWith(otherActs);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace SomewhatEnhancedDisplay.Config;

public enum ModHoverGuideResistLevelLabelType
{
    LangText = 0,
    Value,
}

public enum ModHealthBarDisplayTarget
{
    None = 0,
    All,
    Elite,
    Boss,
}

public abstract class ModConfigBase<T> where T : ModConfigBase<T>
{
    public string Serialize()
    {
        return JsonConvert.SerializeObject(this, GameIO.formatting, GameIO.jsWriteGame);
    }

    public static T Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, GameIO.jsReadGame);
    }

    public T DeepCopy()
    {
        return Deserialize(Serialize());
    }
} 

public class ModConfig : ModConfigBase<ModConfig>
{
    [JsonProperty("hoverGuide", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigHoverGuide HoverGuide { get; set; } = new();

    public void ResetHoverGuide()
    {
        HoverGuide = new();
    }

    public void ResetHoverGuideGeneral()
    {
        HoverGuide = HoverGuide.DeepCopyWithoutGeneral();
    }

    public void ResetHoverGuideStyle()
    {
        HoverGuide = HoverGuide.DeepCopyWithoutStyle();
    }
}

public class ModConfigHoverGuide : ModConfigBase<ModConfigHoverGuide>
{
    [JsonProperty("zoomScale", DefaultValueHandling = DefaultValueHandling.Include)]
    public float ZoomScale { get; set; } = 1.2f;

    [JsonProperty("horizontalPivot", DefaultValueHandling = DefaultValueHandling.Include)]
    public float HorizontalPivot { get; set; } = 0.5f;

    [JsonProperty("verticalPivot", DefaultValueHandling = DefaultValueHandling.Include)]
    public float VerticalPivot { get; set; } = 0.8f;

    [JsonProperty("colorSet", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigHoverGuideColorSet ColorSet { get; set; } = new();

    [JsonProperty("styles", DefaultValueHandling = DefaultValueHandling.Include, ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<ModConfigHoverGuideStyle> Styles { get; private set; } = [
        new ModConfigHoverGuideStyle(),
        ModConfigHoverGuideStyle.CreateDisplayAll(),
    ];

    [JsonProperty("currentStyleIndex", DefaultValueHandling = DefaultValueHandling.Include, ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private int CurrentStyleIndex { get; set; } = 0;

    [JsonIgnore]
    public ModConfigHoverGuideStyle CurrentStyle
    {
        get
        {
            if (Styles.Count == 0)
            {
                Styles.Add(new ModConfigHoverGuideStyle());
            }
            CurrentStyleIndex = Math.Min(CurrentStyleIndex, Styles.Count - 1);
            return Styles[CurrentStyleIndex];
        }
    }

    public void AdvanceStyle()
    {
        var index = CurrentStyleIndex + 1;
        if (index >= Styles.Count)
        {
            index = 0;
        }
        CurrentStyleIndex = index;
    }

    public ModConfigHoverGuide DeepCopyWithoutGeneral()
    {
        return new ModConfigHoverGuide()
        {
            Styles = [.. Styles.Select(s => s.DeepCopy())],
            CurrentStyleIndex = CurrentStyleIndex,
        };
    }

    public ModConfigHoverGuide DeepCopyWithoutStyle()
    {
        var newConfig = DeepCopy();
        var defaultConfig = new ModConfigHoverGuide();
        newConfig.Styles = [.. defaultConfig.Styles.Select(s => s.DeepCopy())];
        newConfig.CurrentStyleIndex = defaultConfig.CurrentStyleIndex;
        return newConfig;
    }
}

public class ModConfigHoverGuideColorSet : ModConfigBase<ModConfigHoverGuide>
{
    [JsonProperty("mainTextColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color MainTextColor { get; set; } = new(0.9028f, 0.8804f, 0.8354f); // #E6E1D5FF

    [JsonProperty("subTextColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color SubTextColor { get; set; } = new(0.703f, 0.681f, 0.636f); // #B3AEA2FF

    [JsonProperty("hpLabelColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color HPLabelColor { get; set; } = new(0.872f, 0.371f, 0.335f); // #DE5F55FF

    [JsonProperty("hpValueColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color HPValueColor { get; set; } = new(0.982f, 0.701f, 0.665f); // #FAB3AAFF

    [JsonProperty("manaLabelColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color ManaLabelColor { get; set; } = new(0.375f, 0.606f, 0.988f); // #609BFCFF

    [JsonProperty("manaValueColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color ManaValueColor { get; set; } = new(0.665f, 0.806f, 0.838f); // #AACED6FF

    [JsonProperty("staminaLabelColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color StaminaLabelColor { get; set; } = new(0.848f, 0.722f, 0.285f); // #D8B849FF

    [JsonProperty("staminaValueColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color StaminaValueColor { get; set; } = new(0.848f, 0.82f, 0.635f); // #D8D1A2FF

    [JsonProperty("resistLabelColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color ResistLabelColor { get; set; } = new(0.375f, 0.738f, 0.626f); // #60BCA0FF

    [JsonProperty("negativeResistLabelColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color NegativeResistLabelColor { get; set; } = new(0.822f, 0.431f, 0.395f); // #D26E65FF

    [JsonProperty("noneResistLabelColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color NoneResistLabelColor { get; set; } = new(0.7f, 0.7f, 0.7f); // #B2B2B2FF

    [JsonProperty("healthBarBGColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color HealthBarBGColor { get; set; } = new(0.2f, 0.1f, 0.1f); // #331A1AFF

    [JsonProperty("healthBarFGColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color HealthBarFGColor { get; set; } = new(0.212f, 0.459f, 0.184f); // #36752FFF

    [JsonProperty("healthBarFGDamageColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color HealthBarFGDamageColor { get; set; } = new(0.6f, 0.6f, 0.6f); // #999999FF

    [JsonProperty("healthBarLowValueFGColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color HealthBarLowValueFGColor { get; set; } = new(0.485f, 0.189f, 0.104f); // #7C301BFF

    [JsonProperty("healthBarTextColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color HealthBarTextColor { get; set; } = new(0.8f, 0.8f, 0.8f); // #CCCCCCFF

    [JsonProperty("healthBarLowValueTextColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color HealthBarLowValueTextColor { get; set; } = new(0.872f, 0.371f, 0.335f); // #DE5F55FF

    [JsonProperty("rarityCrudeColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color RarityCrudeColor { get; set; } = new(0.62f, 0.62f, 0.62f); // #9E9E9EFF

    [JsonProperty("rarityNormalColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color RarityNormalColor { get; set; } = new(0.9028f, 0.8804f, 0.8354f); // #E6E1D5FF

    [JsonProperty("raritySuperiorColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color RaritySuperiorColor { get; set; } = new(0.412f, 0.797f, 0.526f); // #69CB86FF

    [JsonProperty("rarityLegendaryColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color RarityLegendaryColor { get; set; } = new(0.83f, 0.762f, 0.411f); // #D4C269FF

    [JsonProperty("rarityMythicalColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color RarityMythicalColor { get; set; } = new(0.888f, 0.525f, 0.364f); // #E2865DFF

    [JsonProperty("rarityArtifactColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color RarityArtifactColor { get; set; } = new(0.64f, 0.614f, 0.891f); // #A39DE3FF

    [JsonProperty("fressnessValueColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color FressnessValueColor { get; set; } = new(0.375f, 0.738f, 0.626f); // #60BCA0FF

    [JsonProperty("fressnessLowValueColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color FressnessLowValueColor { get; set; } = new(0.822f, 0.431f, 0.395f); // #D26E65FF
}

public class ModConfigHoverGuideStyle : ModConfigBase<ModConfigHoverGuideStyle>
{
    [JsonProperty("chara", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigHoverGuideStyleChara Chara { get; set; } = new();

    [JsonProperty("thing", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigHoverGuideStyleThing Thing { get; set; } = new();

    public static ModConfigHoverGuideStyle CreateDisplayAll()
    {
        return new()
        {
            Chara = ModConfigHoverGuideStyleChara.CreateDisplayAll(),
            Thing = ModConfigHoverGuideStyleThing.CreateDisplayAll(),
        };
    }
}

public class ModConfigHoverGuideStyleChara : ModConfigBase<ModConfigHoverGuideStyleChara>
{
    [JsonProperty("displayType", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayType { get; set; } = true;

    [JsonProperty("displayLv", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayLv { get; set; } = true;

    [JsonProperty("displayHealthBar", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayHealthBar { get; set; } = true;

    [JsonProperty("displayGender", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayGender { get; set; } = false;

    [JsonProperty("displayAge", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayAge { get; set; } = false;

    [JsonProperty("displayRace", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayRace { get; set; } = false;

    [JsonProperty("displayJobTactics", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayJobTactics { get; set; } = false;

    [JsonProperty("displayHobby", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayHobby { get; set; } = true;

    [JsonProperty("displayAffinity", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayAffinity { get; set; } = false;

    [JsonProperty("displayFavorite", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayFavorite { get; set; } = true;

    [JsonProperty("displayHP", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayHP { get; set; } = true;

    [JsonProperty("displayMana", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayMana { get; set; } = true;

    [JsonProperty("displayStamina", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayStamina { get; set; } = true;

    [JsonProperty("displayDVPV", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayDVPV { get; set; } = true;

    [JsonProperty("displaySpeed", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplaySpeed { get; set; } = true;

    [JsonProperty("displayExp", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayExp { get; set; } = false;

    [JsonProperty("displayMainElement", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayMainElement { get; set; } = false;

    [JsonProperty("displayPrimaryAttributes", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayPrimaryAttributes { get; set; } = false;

    [JsonProperty("displayFeat", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayFeat { get; set; } = false;

    [JsonProperty("displayFeatValue", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayFeatValue { get; set; } = true;

    [JsonProperty("displayAct", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayAct { get; set; } = true;

    [JsonProperty("displayActParty", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayActParty { get; set; } = true;

    [JsonProperty("displayResist", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayResist { get; set; } = false;

    [JsonProperty("displayResistValue", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayResistValue { get; set; } = true;

    [JsonProperty("resistLevelLabelType", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModHoverGuideResistLevelLabelType ResistLevelLabelType { get; set; } = ModHoverGuideResistLevelLabelType.LangText;

    [JsonProperty("displayStats", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayStats { get; set; } = true;

    [JsonProperty("displayStatsValue", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayStatsValue { get; set; } = false;

    [JsonProperty("enableMimicry", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool EnableMimicry { get; set; } = true;

    [JsonProperty("healthBar", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigHoverGuideHealthBar HealthBar { get; private set; } = new();

    public static ModConfigHoverGuideStyleChara CreateDisplayAll()
    {
        return new()
        {
            DisplayType = true,
            DisplayLv = true,
            DisplayHealthBar = true,
            DisplayGender = true,
            DisplayAge = true,
            DisplayRace = true,
            DisplayJobTactics = true,
            DisplayHobby = true,
            DisplayAffinity = true,
            DisplayFavorite = true,
            DisplayHP = true,
            DisplayMana = true,
            DisplayStamina = true,
            DisplayDVPV = true,
            DisplaySpeed = true,
            DisplayExp = true,
            DisplayMainElement = true,
            DisplayPrimaryAttributes = true,
            DisplayFeat = true,
            DisplayFeatValue = true,
            DisplayAct = true,
            DisplayActParty = true,
            DisplayResist = true,
            DisplayResistValue = true,
            DisplayStats = true,
            DisplayStatsValue = true,
            EnableMimicry = false,
            HealthBar = ModConfigHoverGuideHealthBar.CreateDisplayAll()
        };
    }
}

public class ModConfigHoverGuideHealthBar : ModConfigBase<ModConfigHoverGuideHealthBar>
{
    [JsonProperty("width", DefaultValueHandling = DefaultValueHandling.Include)]
    public int Width { get; set; } = 300;

    [JsonProperty("displayValue", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayValue { get; set; } = true;

    [JsonProperty("useAnimation", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool UseAnimation { get; set; } = true;

    [JsonProperty("displayForEnemy", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigHealthBarDisplay DisplayForEnemy { get; set; } = new()
    {
        Target = ModHealthBarDisplayTarget.All,
        NotInCombat = true,
        InFullHealth = false,
    };

    [JsonProperty("displayForNetural", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigHealthBarDisplay DisplayForNetural { get; set; } = new()
    {
        Target = ModHealthBarDisplayTarget.None,
        NotInCombat = true,
        InFullHealth = false,
    };

    [JsonProperty("displayForFriend", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigHealthBarDisplay DisplayForFriend { get; set; } = new()
    {
        Target = ModHealthBarDisplayTarget.None,
        NotInCombat = true,
        InFullHealth = false,
    };

    [JsonProperty("displayForAlly", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigHealthBarDisplay DisplayForAlly { get; set; } = new()
    {
        Target = ModHealthBarDisplayTarget.None,
        NotInCombat = true,
        InFullHealth = false,
    };

    public static ModConfigHoverGuideHealthBar CreateDisplayAll()
    {
        return new()
        {
            DisplayValue = true,
            DisplayForEnemy = ModConfigHealthBarDisplay.CreateDisplayAll(),
            DisplayForNetural = ModConfigHealthBarDisplay.CreateDisplayAll(),
            DisplayForFriend = ModConfigHealthBarDisplay.CreateDisplayAll(),
            DisplayForAlly = ModConfigHealthBarDisplay.CreateDisplayAll(),
        };
    }
}

public class ModConfigHealthBarDisplay : ModConfigBase<ModConfigHealthBarDisplay>
{
    [JsonProperty("target", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModHealthBarDisplayTarget Target { get; set; } = ModHealthBarDisplayTarget.All;

    [JsonProperty("notInCombat", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool NotInCombat { get; set; } = true;

    [JsonProperty("inFullHealth", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool InFullHealth { get; set; } = false;

    public static ModConfigHealthBarDisplay CreateDisplayAll()
    {
        return new()
        {
            Target = ModHealthBarDisplayTarget.All,
            NotInCombat = true,
            InFullHealth = true,
        };
    }
}

public class ModConfigHoverGuideStyleThing : ModConfigBase<ModConfigHoverGuideStyleThing>
{
    [JsonProperty("displayLv", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayLv { get; set; } = false;

    [JsonProperty("displayFressness", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayFressness { get; set; } = false;

    [JsonProperty("displayLockLv", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayLockLv { get; set; } = false;

    [JsonProperty("useRarityColor", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool UseRarityColor { get; set; } = true;

    public static ModConfigHoverGuideStyleThing CreateDisplayAll()
    {
        return new()
        {
            DisplayLv = true,
            DisplayFressness = true,
            DisplayLockLv = true,
            UseRarityColor = true,
        };
    }
}

public class ColorConverter : JsonConverter<Color?>
{
    public override bool CanWrite => true;

    public override Color? ReadJson(JsonReader reader, Type objectType, Color? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value is null && Nullable.GetUnderlyingType(objectType) is not null)
        {
            return null;
        }
        if (reader.Value is not string colorString)
        {
            throw new JsonSerializationException($"Unexpected JSON format in ColorConverter: {reader.Value}");
        }
        if (!ColorUtility.TryParseHtmlString(colorString, out var color))
        {
            throw new JsonSerializationException($"Unexpected color format in ColorConverter: {colorString}");
        }
        return color;
    }

    public override void WriteJson(JsonWriter writer, Color? value, JsonSerializer serializer)
    {
        writer.WriteValue(value is Color color ? $"#{ColorUtility.ToHtmlStringRGBA(color)}" : null);
    }
}

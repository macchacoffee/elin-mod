using System;
using System.Collections.Generic;
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
}

public class ModConfig
{
    [JsonProperty("hoverGuide", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigHoverGuide HoverGuide { get; } = new();
}

public class ModConfigHoverGuide
{
    [JsonProperty("scale", DefaultValueHandling = DefaultValueHandling.Include)]
    public float Scale { get; set; } = 1.2f;

    [JsonProperty("mainTextColor", DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color? MainTextColor { get; set; } = null;

    [JsonProperty("subTextColor", DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color? SubTextColor { get; set; } = null;

    [JsonProperty("hpColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color HPColor { get; set; } = new(0.872f, 0.371f, 0.335f); // #DE5F55FF

    [JsonProperty("hpLightenColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color HPLightenColor { get; set; } = new(0.982f, 0.701f, 0.665f); // #FAB3AAFF

    [JsonProperty("manaColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color ManaColor { get; set; } = new(0.375f, 0.606f, 0.988f); // #609BFCFF

    [JsonProperty("manaLightenColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color ManaLightenColor { get; set; } = new(0.665f, 0.806f, 0.838f); // #AACED6FF

    [JsonProperty("staminaColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color StaminaColor { get; set; } = new(0.848f, 0.722f, 0.285f); // #D8B849FF

    [JsonProperty("staminaLightenColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color StaminaLightenColor { get; set; } = new(0.848f, 0.82f, 0.635f); // #D8D1A2FF

    [JsonProperty("resistColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color ResistColor { get; set; } = new(0.375f, 0.738f, 0.626f); // #60BCA0FF

    [JsonProperty("negativeResistColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color NegativeResistColor { get; set; } = new(0.822f, 0.431f, 0.395f); // #D26E65FF

    [JsonProperty("noneResistColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color NoneResistColor { get; set; } = new(0.7f, 0.7f, 0.7f); // #B2B2B2FF

    [JsonProperty("healthBarBGColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color HealthBarBGColor { get; set; } = new(0.2f, 0.1f, 0.1f); // #331A1AFF

    [JsonProperty("healthBarFGDamageColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color HealthBarFGDamageColor { get; set; } = new(0.6f, 0.6f, 0.6f); // #999999FF

    [JsonProperty("healthBarFGColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color HealthBarFGColor { get; set; } = new(0.212f, 0.459f, 0.184f); // #36752FFF

    [JsonProperty("healthBarLowValueFGColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color HealthBarLowValueFGColor { get; set; } = new(0.485f, 0.189f, 0.104f); // #7C301BFF

    [JsonProperty("healthBarValueTextColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color HealthBarValueTextColor { get; set; } = new(0.8f, 0.8f, 0.8f); // #CCCCCCFF

    [JsonProperty("healthBarLowValueTextColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color HealthBarLowValueTextColor { get; set; } = new(0.872f, 0.371f, 0.335f); // #DE5F55FF

    [JsonProperty("fressnessValueColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color FressnessValueColor { get; set; } = new(0.375f, 0.738f, 0.626f); // #60BCA0FF

    [JsonProperty("fressnessLowValueColor", DefaultValueHandling = DefaultValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color FressnessLowValueColor { get; set; } = new(0.822f, 0.431f, 0.395f); // #D26E65FF

    [JsonProperty("rariryCrudeColor", DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color? RariryCrudeColor { get; set; } = new(0.62f, 0.62f, 0.62f); // #9E9E9EFF

    [JsonProperty("rariryNormalColor", DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color? RariryNormalColor { get; set; } = null;

    [JsonProperty("rarirySuperiorColor", DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color? RarirySuperiorColor { get; set; } = new(0.412f, 0.797f, 0.526f); // #69CB86FF

    [JsonProperty("rariryLegendaryColor", DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color? RariryLegendaryColor { get; set; } = new(0.83f, 0.762f, 0.411f); // #D4C269FF

    [JsonProperty("rariryMythicalColor", DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color? RariryMythicalColor { get; set; } = new(0.888f, 0.525f, 0.364f); // #E2865DFF

    [JsonProperty("rariryArtifactColor", DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
    [JsonConverter(typeof(ColorConverter))]
    public Color? RariryArtifactColor { get; set; } = new(0.64f, 0.614f, 0.891f); // #A39DE3FF

    [JsonProperty("profiles", DefaultValueHandling = DefaultValueHandling.Include, ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<ModConfigHoverGuideProfile> Profiles { get; private set; } = [
        new ModConfigHoverGuideProfile(),
        ModConfigHoverGuideProfile.CreateDisplayAll(),
    ];

    [JsonProperty("currentProfileIndex", DefaultValueHandling = DefaultValueHandling.Include, ObjectCreationHandling = ObjectCreationHandling.Replace)]
    private int CurrentProfileIndex { get; set; } = 0;

    [JsonIgnore]
    public ModConfigHoverGuideProfile CurrentProfile
    {
        get
        {
            if (Profiles.Count == 0)
            {
                Profiles.Add(new ModConfigHoverGuideProfile());
            }
            CurrentProfileIndex = Math.Min(CurrentProfileIndex, Profiles.Count - 1);
            return Profiles[CurrentProfileIndex];
        }
    }

    public void AdvanceProfile()
    {
        var index = CurrentProfileIndex + 1;
        if (index >= Profiles.Count)
        {
            index = 0;
        }
        CurrentProfileIndex = index;
    }
}

public class ModConfigHoverGuideProfile
{
    [JsonProperty("chara", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigHoverGuideProfileChara Chara { get; set; } = new();

    [JsonProperty("thing", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigHoverGuideProfileThing Thing { get; set; } = new();

    public static ModConfigHoverGuideProfile CreateDisplayAll()
    {
        return new()
        {
            Chara = ModConfigHoverGuideProfileChara.CreateDisplayAll(),
            Thing = ModConfigHoverGuideProfileThing.CreateDisplayAll(),
        };
    }
}

public class ModConfigHoverGuideProfileChara
{
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

    [JsonProperty("healthBar", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigHoverGuideHealthBar HealthBar { get; private set; } = new();

    public static ModConfigHoverGuideProfileChara CreateDisplayAll()
    {
        return new()
        {
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
            HealthBar = ModConfigHoverGuideHealthBar.CreateDisplayAll()
        };
    }
}

public class ModConfigHoverGuideHealthBar
{
    [JsonProperty("displayValue", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayValue { get; set; } = true;

    [JsonProperty("width", DefaultValueHandling = DefaultValueHandling.Include)]
    public int Width { get; set; } = 300;

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

public record ModConfigHealthBarDisplay
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

public class ModConfigHoverGuideProfileThing
{
    [JsonProperty("displayLv", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayLv { get; set; } = false;

    [JsonProperty("displayFressness", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayFressness { get; set; } = false;

    [JsonProperty("displayLockLv", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayLockLv { get; set; } = false;

    [JsonProperty("useRarityColor", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool UseRarityColor { get; set; } = true;

    public static ModConfigHoverGuideProfileThing CreateDisplayAll()
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

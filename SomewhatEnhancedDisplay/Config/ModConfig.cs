using System.Collections.Generic;
using Newtonsoft.Json;

namespace SomewhatEnhancedDisplay.Config;

public class ModConfig
{
    [JsonProperty("hoverGuide", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModConfigHoverGuide HoverGuide { get; } = new();
}

public class ModConfigHoverGuide
{
    [JsonProperty("scale", DefaultValueHandling = DefaultValueHandling.Include)]
    public float Scale { get; set; } = 1.2f;

    [JsonProperty("profiles", DefaultValueHandling = DefaultValueHandling.Include, ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<ModHoverGuideProfile> profiles { get; private set; } = [
        new ModHoverGuideProfile(),
        new ModHoverGuideProfile()
    ];

    public ModHoverGuideProfile CurrentProfile
    {
        get
        {
            // TODO
            return profiles[0];
        }
    }
}

public class ModHoverGuideProfile
{
    [JsonProperty("displayLv", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayLv { get; set; } = true;

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

    [JsonProperty("DisplayExp", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayExp { get; set; } = false;

    [JsonProperty("displayMainElement", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayMainElement { get; set; } = false;

    [JsonProperty("displayPrimaryAttributes", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayPrimaryAttributes { get; set; } = false;

    [JsonProperty("displayFeat", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayFeat { get; set; } = false;

    [JsonProperty("displayAct", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayAct { get; set; } = true;

    [JsonProperty("displayResist", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayResist { get; set; } = false;

    [JsonProperty("displayHealthBar", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool DisplayHealthBar { get; set; } = true;

    [JsonProperty("healthBar", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModHoverGuideHealthBar HealthBar { get; private set; } = new ModHoverGuideHealthBar();
}

public class ModHoverGuideHealthBar
{
    [JsonProperty("width", DefaultValueHandling = DefaultValueHandling.Include)]
    public int Width { get; set; } = 300;

    [JsonProperty("displayForEnemy", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModHealthBarDisplay DisplayForEnemy { get; set; } = new(target: ModHealthBarDisplayTarget.All, notInCombat: true, inFullHealth: true);

    [JsonProperty("displayForNetural", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModHealthBarDisplay DisplayForNetural { get; set; } = new(target: ModHealthBarDisplayTarget.None, notInCombat: true, inFullHealth: true);

    [JsonProperty("displayForFriend", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModHealthBarDisplay DisplayForFriend { get; set; } = new(target: ModHealthBarDisplayTarget.None, notInCombat: true, inFullHealth: true);

    [JsonProperty("displayForAlly", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModHealthBarDisplay DisplayForAlly { get; set; } = new(target: ModHealthBarDisplayTarget.None, notInCombat: true, inFullHealth: true);
}

public enum ModHealthBarDisplayTarget
{
    None = 0,
    All,
    Elite,
}

public record ModHealthBarDisplay
{
    [JsonProperty("target", DefaultValueHandling = DefaultValueHandling.Include)]
    public ModHealthBarDisplayTarget Target { get; set; }

    [JsonProperty("notInCombat", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool NotInCombat { get; set; }

    [JsonProperty("inFullHealth", DefaultValueHandling = DefaultValueHandling.Include)]
    public bool InFullHealth { get; set; }

    public ModHealthBarDisplay(ModHealthBarDisplayTarget target, bool notInCombat, bool inFullHealth)
    {
        Target = target;
        NotInCombat = notInCombat;
        InFullHealth = inFullHealth;
    }
}

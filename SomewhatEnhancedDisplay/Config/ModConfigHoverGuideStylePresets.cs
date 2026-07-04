namespace SomewhatEnhancedDisplay.Config;

public static class ModConfigHoverGuideStylePresets
{
    public static ModConfigHoverGuideStyle Minimum()
    {
        return new()
        {
            Chara = new()
            {
                DisplayType = true,
                DisplayLv = true,
                DisplayHealthBar = true,
                DisplayGender = false,
                DisplayAge = false,
                DisplayRace = false,
                DisplayJobTactics = false,
                DisplayHobby = false,
                DisplayHobbyAlways = false,
                DisplayAffinity = false,
                DisplayFavorite = false,
                DisplayFavoriteAlways = false,
                DisplayHP = false,  
                DisplayMana = false,
                DisplayStamina = false,
                DisplayDVPV = false,
                DisplaySpeed = false,
                DisplayExp = false,
                DisplayMainElement = false,
                DisplayPrimaryAttributes = false,
                DisplayFeat = false,
                DisplayFeatValue = false,
                DisplayAct = false,
                DisplayActParty = false,
                DisplayResist = false,
                DisplayResistValue = false,
                DisplayStats = true,
                DisplayStatsValue = false,
                DisableShadowform = false,
                DisableMimicry = false,
                HealthBar = new()
                {
                    DisplayValue = true,
                    DisplayForEnemy = new()
                    {
                        Target = ModHealthBarDisplayTarget.All,
                        NotInCombat = true,
                        InFullHealth = true,
                    },
                    DisplayForNetural = new()
                    {
                        Target = ModHealthBarDisplayTarget.All,
                        NotInCombat = true,
                        InFullHealth = false,
                    },
                    DisplayForFriend = new()
                    {
                        Target = ModHealthBarDisplayTarget.All,
                        NotInCombat = true,
                        InFullHealth = false,
                    },
                    DisplayForAlly = new()
                    {
                        Target = ModHealthBarDisplayTarget.All,
                        NotInCombat = true,
                        InFullHealth = false,
                    },
                }
            },
            Thing = new()
            {
                DisplayLv = false,
                DisplayMaterial = false,
                DisplayFressness = false,
                DisplayLockLv = false,
                UseRarityColor = true,
            }
        };
    }

   public static ModConfigHoverGuideStyle Default()
    {
        return new();
    }

    public static ModConfigHoverGuideStyle Maximal()
    {
        return new()
        {
            Chara = new()
            {
                DisplayType = true,
                DisplayLv = true,
                DisplayHealthBar = true,
                DisplayGender = true,
                DisplayAge = true,
                DisplayRace = true,
                DisplayJobTactics = true,
                DisplayHobby = true,
                DisplayHobbyAlways = true,
                DisplayAffinity = true,
                DisplayFavorite = true,
                DisplayFavoriteAlways = true,
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
                DisableShadowform = true,
                DisableMimicry = true,
                HealthBar = new()
                {
                    DisplayValue = true,
                    DisplayForEnemy = new()
                    {
                        Target = ModHealthBarDisplayTarget.All,
                        NotInCombat = true,
                        InFullHealth = true,
                    },
                    DisplayForNetural = new()
                    {
                        Target = ModHealthBarDisplayTarget.All,
                        NotInCombat = true,
                        InFullHealth = true,
                    },
                    DisplayForFriend = new()
                    {
                        Target = ModHealthBarDisplayTarget.All,
                        NotInCombat = true,
                        InFullHealth = true,
                    },
                    DisplayForAlly = new()
                    {
                        Target = ModHealthBarDisplayTarget.All,
                        NotInCombat = true,
                        InFullHealth = true,
                    },
                }
            },
            Thing = new()
            {
                DisplayLv = true,
                DisplayMaterial = true,
                DisplayFressness = true,
                DisplayLockLv = true,
                UseRarityColor = true,
            }
        };
    }
}


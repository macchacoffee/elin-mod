namespace SomewhatEnhancedDisplay.Config;

internal static class ModConfigHoverGuideStylePresets
{
    public static ModConfigHoverGuideStyle Minimum()
    {
        return new()
        {
            Chara = new()
            {
                DisplayType = true,
                DisplayLv = true,
                DisplayLvComparison = true,
                DisplayFactionMemberType = true,
                DisplayHeightDifference = true,
                DisplayMilkBaby = true,
                DisplayBounty = true,
                DisplayFaith = ModItemDisplayMode.Show,
                DisplayBloodTaste = ModItemDisplayMode.Show,
                DisplayHealthBar = true,
                DisplayGender = false,
                DisplayAge = false,
                DisplayRace = false,
                DisplayJobTactics = false,
                DisplayHobby = ModItemDisplayMode.Show,
                DisplayAffinity = false,
                DisplayFavorite = ModItemDisplayMode.Show,
                DisplayHP = false,
                DisplayMana = false,
                DisplayStamina = false,
                DisplayDVPV = false,
                DisplaySpeed = false,
                DisplayLuck = false,
                DisplayExp = false,
                DisplayMainElement = false,
                DisplayExpForOnlyAlly = false,
                DisplayPrimaryAttributes = false,
                DisplayFeat = false,
                DisplayFeatValue = true,
                DisplayAct = false,
                DisplayActParty = true,
                DisplayResist = false,
                DisplayResistValue = true,
                GroupResistByLavel = true,
                UseShortResistLavelLabel = false,
                DisplayNoneResistLevel = false,
                DisplayStats = true,
                DisplayStatsValue = true,
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
                DisplayLvComparison = true,
                DisplayFactionMemberType = true,
                DisplayHeightDifference = true,
                DisplayMilkBaby = true,
                DisplayBounty = true,
                DisplayFaith = ModItemDisplayMode.AlwaysShow,
                DisplayBloodTaste = ModItemDisplayMode.AlwaysShow,
                DisplayHealthBar = true,
                DisplayGender = true,
                DisplayAge = true,
                DisplayRace = true,
                DisplayJobTactics = true,
                DisplayHobby = ModItemDisplayMode.AlwaysShow,
                DisplayAffinity = true,
                DisplayFavorite = ModItemDisplayMode.AlwaysShow,
                DisplayHP = true,
                DisplayMana = true,
                DisplayStamina = true,
                DisplayDVPV = true,
                DisplaySpeed = true,
                DisplayLuck = true,
                DisplayExp = true,
                DisplayMainElement = true,
                DisplayExpForOnlyAlly = false,
                DisplayPrimaryAttributes = true,
                DisplayFeat = true,
                DisplayFeatValue = true,
                DisplayAct = true,
                DisplayActParty = true,
                DisplayResist = true,
                DisplayResistValue = true,
                GroupResistByLavel = true,
                UseShortResistLavelLabel = false,
                DisplayNoneResistLevel = false,
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

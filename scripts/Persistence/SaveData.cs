namespace Game.scripts.Persistence;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public sealed class SaveData
{
    [JsonPropertyName("progress")]
    [JsonRequired]
    public SavedProgress Progress { get; set; } = new();

    [JsonPropertyName("playerState")]
    [JsonRequired]
    public SavedPlayerState PlayerState { get; set; } = new();
}

public sealed class SavedProgress
{
    [JsonPropertyName("characters")]
    public List<SavedCharacter> Characters { get; set; } = [];

    [JsonPropertyName("skills")]
    public List<SavedSkill> Skills { get; set; } = [];

    [JsonPropertyName("equipments")]
    public List<SavedEquipment> Equipments { get; set; } = [];

    [JsonPropertyName("loot")]
    public List<SavedLoot> Loot { get; set; } = [];

    [JsonPropertyName("enemyDefeatCounts")]
    public Dictionary<string, int> EnemyDefeatCounts { get; set; } = [];

    [JsonPropertyName("dungeonProgress")]
    public Dictionary<string, SavedDungeonProgress> DungeonProgress { get; set; } = [];
}

public sealed class SavedPlayerState
{
    public const string DefaultDungeon = "forest";

    [JsonPropertyName("selectedDungeon")]
    public string SelectedDungeon { get; set; } = DefaultDungeon;

    [JsonPropertyName("selectedTier")]
    public int SelectedTier { get; set; } = 1;

    public bool EnterDungeon(string name, SavedDungeonProgress progress, int tierCount)
    {
        if (SelectedDungeon == name && progress.IsUnlocked(SelectedTier, tierCount)) return false;
        SelectedDungeon = name;
        SelectedTier = 1;
        return true;
    }

    public bool SelectTier(int tier, SavedDungeonProgress progress, int tierCount)
    {
        if (SelectedTier == tier || !progress.IsUnlocked(tier, tierCount)) return false;
        SelectedTier = tier;
        return true;
    }
}

public sealed class SavedLoot
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public int Amount { get; set; }
}

public sealed class SavedCharacter
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("xp")]
    public int Xp { get; set; }

    [JsonPropertyName("skill1Id")]
    public Guid? Skill1Id { get; set; }

    [JsonPropertyName("skill2Id")]
    public Guid? Skill2Id { get; set; }

    [JsonPropertyName("skill3Id")]
    public Guid? Skill3Id { get; set; }

    [JsonPropertyName("skill4Id")]
    public Guid? Skill4Id { get; set; }

    [JsonPropertyName("helmetId")]
    public Guid? HelmetId { get; set; }

    [JsonPropertyName("bodyArmorId")]
    public Guid? BodyArmorId { get; set; }

    [JsonPropertyName("legArmorId")]
    public Guid? LegArmorId { get; set; }

    [JsonPropertyName("weaponId")]
    public Guid? WeaponId { get; set; }

    [JsonPropertyName("accessory1Id")]
    public Guid? Accessory1Id { get; set; }

    [JsonPropertyName("accessory2Id")]
    public Guid? Accessory2Id { get; set; }
}

public sealed class SavedSkill
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public sealed class SavedEquipment
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

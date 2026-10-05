namespace Game.scripts.Persistence;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public sealed class SaveData
{
    [JsonPropertyName("characters")]
    public List<SavedCharacter> Characters { get; set; } = [];

    [JsonPropertyName("skills")]
    public List<SavedSkill> Skills { get; set; } = [];
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
}

public sealed class SavedSkill
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

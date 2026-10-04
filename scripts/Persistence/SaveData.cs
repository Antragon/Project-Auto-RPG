namespace Game.scripts.Persistence;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public sealed class SaveData
{
    [JsonPropertyName("characters")]
    public List<SavedCharacter> Characters { get; set; } = [];
}

public sealed class SavedCharacter
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("xp")]
    public int Xp { get; set; }

    [JsonPropertyName("skill1")]
    public SavedSkill? Skill1 { get; set; }

    [JsonPropertyName("skill2")]
    public SavedSkill? Skill2 { get; set; }

    [JsonPropertyName("skill3")]
    public SavedSkill? Skill3 { get; set; }

    [JsonPropertyName("skill4")]
    public SavedSkill? Skill4 { get; set; }
}

public sealed class SavedSkill
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

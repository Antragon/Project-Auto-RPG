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
}
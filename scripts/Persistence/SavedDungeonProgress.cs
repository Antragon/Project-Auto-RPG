namespace Game.scripts.Persistence;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public sealed class SavedDungeonProgress
{
    public const int DefeatsToUnlock = 10;

    // Keep the existing save key; its values now count completed encounters.
    [JsonPropertyName("defeats")]
    public Dictionary<int, int> Defeats { get; set; } = [];

    [JsonPropertyName("encounteredEnemies")]
    public HashSet<string>? EncounteredEnemies { get; set; }

    public int GetDefeats(int tier) => Defeats.GetValueOrDefault(tier);

    public bool IsUnlocked(int tier, int tierCount)
    {
        if (tier < 1 || tier > tierCount) return false;
        for (var previous = 1; previous < tier; previous++)
        {
            if (GetDefeats(previous) < DefeatsToUnlock) return false;
        }

        return true;
    }

    public void RecordDefeat(int tier, int tierCount)
    {
        if (!IsUnlocked(tier, tierCount)) return;
        var defeats = GetDefeats(tier);
        if (defeats < DefeatsToUnlock) Defeats[tier] = Math.Min(DefeatsToUnlock, defeats + 1);
    }
}

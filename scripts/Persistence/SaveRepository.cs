namespace Game.scripts.Persistence;

using System;
using System.Collections.Generic;
using System.Text.Json;
using Dungeon;
using Extensions;
using Godot;

public sealed class SaveRepository
{
    public const string SavePath = "user://save.json";

    private const string CharacterName = "knight";

    public SaveData SaveData { get; } = LoadOrCreate();

    private static SaveData LoadOrCreate()
    {
        if (!FileAccess.FileExists(SavePath))
        {
            return CreateInitialSave();
        }

        try
        {
            var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
            var saveData = JsonSerializer.Deserialize<SaveData>(file.GetAsText());
            file.Close();

            if (saveData?.Progress is not { } progress || saveData.PlayerState is null
                || progress.Characters is null || progress.Skills is null || progress.Equipments is null
                || progress.Loot is null || progress.EnemyDefeatCounts is null || progress.DungeonProgress is null)
            {
                throw new JsonException("Save file contains invalid game data.");
            }

            saveData.PlayerState.Team ??= [];
            foreach (var (name, dungeonProgress) in progress.DungeonProgress)
            {
                if (dungeonProgress.EncounteredEnemies is not null) continue;
                dungeonProgress.EncounteredEnemies = [];
                var dungeon = DungeonDataRepository.Load(name);
                if (dungeon is null) continue;
                for (var index = 0; index < dungeon.Tiers.Count; index++)
                {
                    if (dungeonProgress.GetDefeats(index + 1) == 0) continue;
                    foreach (var enemy in dungeon.Tiers[index].Enemies)
                    {
                        if (progress.EnemyDefeatCounts.GetValueOrDefault(enemy.Name) > 0)
                            dungeonProgress.EncounteredEnemies.Add(enemy.Name);
                    }
                }
            }
            return saveData;
        }
        catch (Exception exception)
        {
            GD.PushError($"Could not load save file: {exception.Message}");
            return CreateInitialSave();
        }
    }

    private static SaveData CreateInitialSave()
    {
        return new SaveData
        {
            Progress = new SavedProgress
            {
                Characters =
                [
                    new SavedCharacter { Name = CharacterName },
                ],
            },
        };
    }
}

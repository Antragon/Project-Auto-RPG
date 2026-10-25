namespace Game.scripts.Persistence;

using System;
using System.Text.Json;
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

            if (saveData?.Characters is null || saveData.Skills is null || saveData.Equipments is null)
            {
                throw new JsonException("Save file contains invalid character, skill, or equipment data.");
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
            Characters =
            [
                new SavedCharacter { Name = CharacterName },
            ],
        };
    }
}

namespace Game.scripts.Persistence;

using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;

public sealed class SaveRepository
{
    private const string SavePath = "user://save.json";
    private const string CharacterName = "shinobi";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    private readonly SaveData _saveData = LoadOrCreate();

    public IReadOnlyList<SavedCharacter> Characters => _saveData.Characters;

    private static SaveData LoadOrCreate()
    {
        if (!FileAccess.FileExists(SavePath))
        {
            var initialSave = CreateInitialSave();
            Write(initialSave);
            return initialSave;
        }

        try
        {
            var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);

            var saveData = JsonSerializer.Deserialize<SaveData>(file.GetAsText());
            file.Close();

            if (saveData?.Characters is null)
            {
                throw new JsonException("Save file contains invalid character data.");
            }

            return saveData;
        }
        catch (Exception exception)
        {
            GD.PushError($"Could not load save file: {exception.Message}");

            var initialSave = CreateInitialSave();
            Write(initialSave);
            return initialSave;
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

    private static void Write(SaveData saveData)
    {
        var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        if (file is null)
        {
            GD.PushError("Could not open save file for writing.");
            return;
        }

        file.StoreString(JsonSerializer.Serialize(saveData, JsonOptions));
        file.Close();
    }
}

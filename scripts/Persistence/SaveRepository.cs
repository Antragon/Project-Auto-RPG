namespace Game.scripts.Persistence;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Extensions;
using Godot;
using Skills;
using Units;

public sealed class SaveRepository
{
    private const string SavePath = "user://save.json";
    private const string CharacterName = "knight";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    private readonly SaveData _saveData = LoadOrCreate();

    public IReadOnlyList<SavedCharacter> Characters => _saveData.Characters;
    public IReadOnlyList<SavedSkill> Skills => _saveData.Skills;

    public void Save(CharacterRoster characterRoster)
    {
        foreach (var (characterName, character) in characterRoster.Characters)
        {
            var savedCharacter = _saveData.Characters.FirstOrDefault(saved => saved.Name == characterName);
            if (savedCharacter is null)
            {
                savedCharacter = new SavedCharacter { Name = characterName };
                _saveData.Characters.Add(savedCharacter);
            }

            savedCharacter.Xp = character.Xp;
            savedCharacter.Skill1Id = SaveSkill(character.Unit.Skill1, savedCharacter.Skill1Id);
            savedCharacter.Skill2Id = SaveSkill(character.Unit.Skill2, savedCharacter.Skill2Id);
            savedCharacter.Skill3Id = SaveSkill(character.Unit.Skill3, savedCharacter.Skill3Id);
            savedCharacter.Skill4Id = SaveSkill(character.Unit.Skill4, savedCharacter.Skill4Id);
        }

        RemoveUnreferencedSkills();
        Write(_saveData);
    }

    private Guid? SaveSkill(SkillData? skillData, Guid? existingSkillId)
    {
        if (skillData is null)
        {
            return null;
        }

        var savedSkill = existingSkillId is { } skillId
            ? _saveData.Skills.FirstOrDefault(skill => skill.Id == skillId)
            : null;
        if (savedSkill is null)
        {
            savedSkill = new SavedSkill();
            _saveData.Skills.Add(savedSkill);
        }

        savedSkill.Name = skillData.Name;
        return savedSkill.Id;
    }

    private void RemoveUnreferencedSkills()
    {
        var referencedSkillIds = _saveData.Characters
            .SelectMany(character => new[]
            {
                character.Skill1Id,
                character.Skill2Id,
                character.Skill3Id,
                character.Skill4Id,
            })
            .Where(skillId => skillId.HasValue)
            .Select(skillId => skillId!.Value)
            .ToHashSet();

        _saveData.Skills.RemoveAll(skill => !referencedSkillIds.Contains(skill.Id));
    }

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

            if (saveData?.Characters is null || saveData.Skills is null)
            {
                throw new JsonException("Save file contains invalid character or skill data.");
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

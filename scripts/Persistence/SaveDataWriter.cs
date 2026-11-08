namespace Game.scripts.Persistence;

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Equipment;
using Extensions;
using Godot;
using Skills;
using Units;

public sealed class SaveDataWriter(
    SaveRepository saveRepository,
    CharacterRepository characterRepository,
    EquipmentRepository equipmentRepository,
    SkillRepository skillRepository) : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    private Task _pendingWrite = Task.CompletedTask;

    public void Dispose()
    {
        _pendingWrite.GetAwaiter().GetResult();
    }

    public void Save()
    {
        SaveCharacters();
        SaveSkills();
        SaveEquipments();
        // Capture mutable game data on the main thread before handing it to a worker.
        var snapshot = CreateSnapshot(saveRepository.SaveData);
        var path = ProjectSettings.GlobalizePath(SaveRepository.SavePath);
        var previousWrite = _pendingWrite;
        _pendingWrite = Task.Run(async () =>
        {
            await previousWrite.ConfigureAwait(false);
            Write(snapshot, path);
        });
    }

    private void SaveCharacters()
    {
        foreach (var (characterId, character) in characterRepository.Characters)
        {
            var savedCharacter = saveRepository.SaveData.Progress.Characters
                .FirstOrDefault(saved => saved.Id == characterId);
            if (savedCharacter is null)
            {
                savedCharacter = new SavedCharacter
                {
                    Id = characterId,
                    Name = character.Unit.UnitData.Name,
                };
                saveRepository.SaveData.Progress.Characters.Add(savedCharacter);
            }

            savedCharacter.Xp = character.Xp;
            savedCharacter.Hp = character.Unit.Hp;
            savedCharacter.Skill1Id = character.Unit.Skill1?.Id;
            savedCharacter.Skill2Id = character.Unit.Skill2?.Id;
            savedCharacter.Skill3Id = character.Unit.Skill3?.Id;
            savedCharacter.Skill4Id = character.Unit.Skill4?.Id;
            savedCharacter.HelmetId = character.Helmet?.Id;
            savedCharacter.BodyArmorId = character.BodyArmor?.Id;
            savedCharacter.LegArmorId = character.LegArmor?.Id;
            savedCharacter.WeaponId = character.Weapon?.Id;
            savedCharacter.Accessory1Id = character.Accessory1?.Id;
            savedCharacter.Accessory2Id = character.Accessory2?.Id;
        }
    }

    private void SaveSkills()
    {
        foreach (var skill in skillRepository.Skills.Values)
        {
            var savedSkill = saveRepository.SaveData.Progress.Skills.FirstOrDefault(saved => saved.Id == skill.Id);
            if (savedSkill is null)
            {
                savedSkill = new SavedSkill { Id = skill.Id };
                saveRepository.SaveData.Progress.Skills.Add(savedSkill);
            }

            savedSkill.Name = skill.SkillData.Name;
        }
    }

    private void SaveEquipments()
    {
        foreach (var equipment in equipmentRepository.Equipments.Values)
        {
            var savedEquipment = saveRepository.SaveData.Progress.Equipments
                .FirstOrDefault(saved => saved.Id == equipment.Id);
            if (savedEquipment is null)
            {
                savedEquipment = new SavedEquipment { Id = equipment.Id };
                saveRepository.SaveData.Progress.Equipments.Add(savedEquipment);
            }

            savedEquipment.Name = equipment.EquipmentData.Name;
        }
    }

    private static SaveData CreateSnapshot(SaveData saveData)
    {
        var progress = saveData.Progress;
        return new SaveData
        {
            PlayerState = new SavedPlayerState
            {
                SelectedDungeon = saveData.PlayerState.SelectedDungeon,
                SelectedTier = saveData.PlayerState.SelectedTier,
                Team = new(saveData.PlayerState.Team),
            },
            Progress = new SavedProgress
            {
                Characters = progress.Characters.Select(character => new SavedCharacter
                {
                    Id = character.Id,
                    Name = character.Name,
                    Xp = character.Xp,
                    Hp = character.Hp,
                    Skill1Id = character.Skill1Id,
                    Skill2Id = character.Skill2Id,
                    Skill3Id = character.Skill3Id,
                    Skill4Id = character.Skill4Id,
                    HelmetId = character.HelmetId,
                    BodyArmorId = character.BodyArmorId,
                    LegArmorId = character.LegArmorId,
                    WeaponId = character.WeaponId,
                    Accessory1Id = character.Accessory1Id,
                    Accessory2Id = character.Accessory2Id,
                }).ToList(),
                Skills = progress.Skills.Select(skill => new SavedSkill
                {
                    Id = skill.Id,
                    Name = skill.Name,
                }).ToList(),
                Equipments = progress.Equipments.Select(equipment => new SavedEquipment
                {
                    Id = equipment.Id,
                    Name = equipment.Name,
                }).ToList(),
                Loot = progress.Loot.Select(loot => new SavedLoot
                {
                    Name = loot.Name,
                    Amount = loot.Amount,
                }).ToList(),
                EnemyDefeatCounts = new(progress.EnemyDefeatCounts),
                DungeonProgress = progress.DungeonProgress.ToDictionary(
                    entry => entry.Key,
                    entry => new SavedDungeonProgress
                    {
                        Defeats = new(entry.Value.Defeats),
                    }),
            },
        };
    }

    private static void Write(SaveData saveData, string path)
    {
        try
        {
            var temporaryPath = path + ".tmp";
            File.WriteAllText(temporaryPath, JsonSerializer.Serialize(saveData, JsonOptions));
            File.Move(temporaryPath, path, overwrite: true);
        }
        catch (Exception exception)
        {
            GD.PushError($"Could not write save file: {exception.Message}");
        }
    }
}

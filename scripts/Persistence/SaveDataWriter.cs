namespace Game.scripts.Persistence;

using System.Linq;
using System.Text.Json;
using Equipment;
using Extensions;
using Godot;
using Skills;
using Units;

public sealed class SaveDataWriter(
    SaveRepository saveRepository,
    CharacterRepository characterRepository,
    EquipmentRepository equipmentRepository,
    SkillRepository skillRepository)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    public void Save()
    {
        SaveCharacters();
        SaveSkills();
        SaveEquipments();
        Write(saveRepository.SaveData);
    }

    private void SaveCharacters()
    {
        foreach (var (characterId, character) in characterRepository.Characters)
        {
            var savedCharacter = saveRepository.SaveData.Characters
                .FirstOrDefault(saved => saved.Id == characterId);
            if (savedCharacter is null)
            {
                savedCharacter = new SavedCharacter
                {
                    Id = characterId,
                    Name = character.Unit.UnitData.Name,
                };
                saveRepository.SaveData.Characters.Add(savedCharacter);
            }

            savedCharacter.Xp = character.Xp;
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
            var savedSkill = saveRepository.SaveData.Skills.FirstOrDefault(saved => saved.Id == skill.Id);
            if (savedSkill is null)
            {
                savedSkill = new SavedSkill { Id = skill.Id };
                saveRepository.SaveData.Skills.Add(savedSkill);
            }

            savedSkill.Name = skill.SkillData.Name;
        }
    }

    private void SaveEquipments()
    {
        foreach (var equipment in equipmentRepository.Equipments.Values)
        {
            var savedEquipment = saveRepository.SaveData.Equipments
                .FirstOrDefault(saved => saved.Id == equipment.Id);
            if (savedEquipment is null)
            {
                savedEquipment = new SavedEquipment { Id = equipment.Id };
                saveRepository.SaveData.Equipments.Add(savedEquipment);
            }

            savedEquipment.Name = equipment.EquipmentData.Name;
        }
    }

    private static void Write(SaveData saveData)
    {
        var file = FileAccess.Open(SaveRepository.SavePath, FileAccess.ModeFlags.Write);
        if (file is null)
        {
            GD.PushError("Could not open save file for writing.");
            return;
        }

        file.StoreString(JsonSerializer.Serialize(saveData, JsonOptions));
        file.Close();
    }
}

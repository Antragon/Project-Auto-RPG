namespace Game.scripts.Units;

using System;
using System.Collections.Generic;
using System.Linq;
using Equipment;
using Persistence;
using Skills;
using EquipmentInstance = Game.scripts.Equipment.Equipment;

public sealed class CharacterRepositoryFactory(
    SaveRepository saveRepository,
    SkillRepository skillRepository,
    SkillAssignments skillAssignments,
    EquipmentRepository equipmentRepository,
    EquipmentAssignments equipmentAssignments,
    EnemyDefeatRepository enemyDefeatRepository,
    EnemyScalingService enemyScalingService)
{
    public CharacterRepository Create()
    {
        var characters = new Dictionary<Guid, Character>();
        var savedCharacters = saveRepository.SaveData.Progress.Characters
            .Select(savedCharacter => (SavedCharacter: savedCharacter, UnitData: UnitDataRepository.Load(savedCharacter.Name)))
            .Where(character => character.UnitData is not null);

        foreach (var (savedCharacter, unitData) in savedCharacters)
        {
            var unit = new Unit(unitData!);
            var character = new Character(savedCharacter.Id, unit, savedCharacter.Xp);
            enemyScalingService.Apply(unit);
            unit.SetHp(unit.MaxHp);
            characters.Add(character.Id, character);

            unit.PropertyChanged += (changedUnit, propertyName) =>
                OnUnitPropertyChanged(character.Id, changedUnit, propertyName);
            unit.Skill1 = LoadSkill(savedCharacter.Skill1Id);
            unit.Skill2 = LoadSkill(savedCharacter.Skill2Id);
            unit.Skill3 = LoadSkill(savedCharacter.Skill3Id);
            unit.Skill4 = LoadSkill(savedCharacter.Skill4Id);
            character.SetEquipment(EquipmentType.Helmet, LoadEquipment(savedCharacter.HelmetId));
            character.SetEquipment(EquipmentType.BodyArmor, LoadEquipment(savedCharacter.BodyArmorId));
            character.SetEquipment(EquipmentType.LegArmor, LoadEquipment(savedCharacter.LegArmorId));
            character.SetEquipment(EquipmentType.Weapon, LoadEquipment(savedCharacter.WeaponId));
            character.SetEquipment(EquipmentType.Accessory1, LoadEquipment(savedCharacter.Accessory1Id));
            character.SetEquipment(EquipmentType.Accessory2, LoadEquipment(savedCharacter.Accessory2Id));
        }

        var characterRepository = new CharacterRepository(characters, enemyDefeatRepository, enemyScalingService);
        foreach (var character in characters.Values)
        {
            foreach (var equipmentType in Enum.GetValues<EquipmentType>())
            {
                if (character.GetEquipment(equipmentType) is { } equipment)
                {
                    equipmentAssignments.Set(equipment.Id, new EquipmentAssignment(character.Id, equipmentType));
                }
            }

            character.PropertyChanged += OnCharacterPropertyChanged;
        }

        return characterRepository;
    }

    private Skill? LoadSkill(Guid? savedSkillId)
    {
        return savedSkillId is { } skillId && skillRepository.TryGetSkill(skillId, out var skill)
            ? skill
            : null;
    }

    private EquipmentInstance? LoadEquipment(Guid? savedEquipmentId)
    {
        return savedEquipmentId is { } equipmentId && equipmentRepository.TryGetEquipment(equipmentId, out var equipment)
            ? equipment
            : null;
    }

    private void OnCharacterPropertyChanged(Character character, string propertyName)
    {
        if (!Enum.TryParse<EquipmentType>(propertyName, out var equipmentType))
        {
            return;
        }

        var assignment = new EquipmentAssignment(character.Id, equipmentType);
        var equipment = character.GetEquipment(equipmentType);
        if (equipment is null)
        {
            equipmentAssignments.Remove(assignment);
        }
        else
        {
            equipmentAssignments.Set(equipment.Id, assignment);
        }

    }

    private void OnUnitPropertyChanged(Guid characterId, Unit unit, string propertyName)
    {
        switch (propertyName)
        {
            case nameof(Unit.Skill1):
                SaveSkillAssignment(new SkillAssignment(characterId, 0), unit.Skill1);
                break;
            case nameof(Unit.Skill2):
                SaveSkillAssignment(new SkillAssignment(characterId, 1), unit.Skill2);
                break;
            case nameof(Unit.Skill3):
                SaveSkillAssignment(new SkillAssignment(characterId, 2), unit.Skill3);
                break;
            case nameof(Unit.Skill4):
                SaveSkillAssignment(new SkillAssignment(characterId, 3), unit.Skill4);
                break;
        }
    }

    private void SaveSkillAssignment(SkillAssignment assignment, Skill? skill)
    {
        if (skill is null)
        {
            skillAssignments.Remove(assignment);
        }
        else
        {
            skillAssignments.Set(skill.Id, assignment);
        }
    }
}

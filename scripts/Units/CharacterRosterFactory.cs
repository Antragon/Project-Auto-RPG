namespace Game.scripts.Units;

using System;
using System.Collections.Generic;
using System.Linq;
using Persistence;
using Skills;

public sealed class CharacterRosterFactory(
    SaveRepository saveRepository,
    SkillAssignments skillAssignments)
{
    public CharacterRoster Create()
    {
        var characters = new Dictionary<string, Character>();
        var savedCharacters = saveRepository.Characters
            .Select(savedCharacter => (SavedCharacter: savedCharacter, UnitData: UnitDataRepository.Load(savedCharacter.Name)))
            .Where(character => character.UnitData is not null);

        foreach (var (savedCharacter, unitData) in savedCharacters)
        {
            var unit = new Unit(unitData!);

            var character = new Character(
                unit,
                savedCharacter.Xp);
            characters.Add(savedCharacter.Name, character);

            unit.PropertyChanged += (u, propertyName) => OnUnitPropertyChanged(u, propertyName, savedCharacter.Name);
            unit.Skill1 = LoadSkill(savedCharacter.Skill1Id);
            unit.Skill2 = LoadSkill(savedCharacter.Skill2Id);
            unit.Skill3 = LoadSkill(savedCharacter.Skill3Id);
            unit.Skill4 = LoadSkill(savedCharacter.Skill4Id);
        }

        return new CharacterRoster(characters);
    }

    private SkillData? LoadSkill(Guid? savedSkillId)
    {
        var savedSkill = savedSkillId is { } skillId
            ? saveRepository.Skills.FirstOrDefault(skill => skill.Id == skillId)
            : null;

        return savedSkill is null
            ? null
            : SkillDataRepository.Load(savedSkill.Name);
    }

    private void OnUnitPropertyChanged(Unit unit, string propertyName, string characterName)
    {
        switch (propertyName)
        {
            case nameof(Unit.Skill1):
                SaveSkillAssignment(
                    new SkillAssignment(characterName, 0),
                    unit.Skill1);
                break;
            case nameof(Unit.Skill2):
                SaveSkillAssignment(
                    new SkillAssignment(characterName, 1),
                    unit.Skill2);
                break;
            case nameof(Unit.Skill3):
                SaveSkillAssignment(
                    new SkillAssignment(characterName, 2),
                    unit.Skill3);
                break;
            case nameof(Unit.Skill4):
                SaveSkillAssignment(
                    new SkillAssignment(characterName, 3),
                    unit.Skill4);
                break;
        }
    }

    private void SaveSkillAssignment(SkillAssignment assignment, SkillData? skillData)
    {
        if (skillData is null)
        {
            skillAssignments.Remove(assignment);
        }
        else
        {
            skillAssignments.Add(skillData, assignment);
        }
    }
}

namespace Game.scripts.Units;

using System;
using System.Collections.Generic;
using System.Linq;
using Persistence;
using Skills;

public sealed class CharacterRepositoryFactory(
    SaveRepository saveRepository,
    SkillRepository skillRepository,
    SkillAssignments skillAssignments)
{
    public CharacterRepository Create()
    {
        var characters = new Dictionary<Guid, Character>();
        var savedCharacters = saveRepository.Characters
            .Select(savedCharacter => (SavedCharacter: savedCharacter, UnitData: UnitDataRepository.Load(savedCharacter.Name)))
            .Where(character => character.UnitData is not null);

        foreach (var (savedCharacter, unitData) in savedCharacters)
        {
            var unit = new Unit(unitData!);
            var character = new Character(savedCharacter.Id, unit, savedCharacter.Xp);
            characters.Add(character.Id, character);

            unit.PropertyChanged += (changedUnit, propertyName) =>
                OnUnitPropertyChanged(character.Id, changedUnit, propertyName);
            unit.Skill1 = LoadSkill(savedCharacter.Skill1Id);
            unit.Skill2 = LoadSkill(savedCharacter.Skill2Id);
            unit.Skill3 = LoadSkill(savedCharacter.Skill3Id);
            unit.Skill4 = LoadSkill(savedCharacter.Skill4Id);
        }

        return new CharacterRepository(characters);
    }

    private Skill? LoadSkill(Guid? savedSkillId)
    {
        return savedSkillId is { } skillId && skillRepository.TryGetSkill(skillId, out var skill)
            ? skill
            : null;
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

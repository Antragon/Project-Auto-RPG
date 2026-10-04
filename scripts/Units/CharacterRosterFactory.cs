namespace Game.scripts.Units;

using System.Collections.Generic;
using System.Linq;
using Persistence;
using Skills;

public sealed class CharacterRosterFactory(SaveRepository saveRepository)
{
    public CharacterRoster Create()
    {
        var characters = new Dictionary<string, Character>();
        var savedCharacters = saveRepository.Characters
            .Select(savedCharacter => (SavedCharacter: savedCharacter, UnitData: UnitDataRepository.Load(savedCharacter.Name)))
            .Where(character => character.UnitData is not null);

        foreach (var (savedCharacter, unitData) in savedCharacters)
        {
            characters.Add(
                savedCharacter.Name,
                new Character(
                    new Unit(unitData!)
                    {
                        Skill1 = LoadSkill(savedCharacter.Skill1),
                        Skill2 = LoadSkill(savedCharacter.Skill2),
                        Skill3 = LoadSkill(savedCharacter.Skill3),
                        Skill4 = LoadSkill(savedCharacter.Skill4),
                    },
                    savedCharacter.Xp));
        }

        return new CharacterRoster(characters);
    }

    private static SkillData? LoadSkill(SavedSkill? savedSkill)
    {
        return savedSkill is null
            ? null
            : SkillDataRepository.Load(savedSkill.Name);
    }
}

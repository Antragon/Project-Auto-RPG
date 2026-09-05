namespace Game.scripts.Units;

using System.Collections.Generic;
using System.Linq;
using Extensions;
using Persistence;

public sealed class CharacterRoster
{
    private readonly Dictionary<string, Character> _characters = [];

    public CharacterRoster(SaveRepository saveRepository)
    {
        var characters = saveRepository.Characters
            .Select(c => (SavedCharacter: c, UnitData: UnitDataRepository.Load(c.Name)))
            .Where(c => c.UnitData is not null);
        foreach (var (savedCharacter, unitData) in characters)
        {
            _characters.Add(savedCharacter.Name, new Character(new Unit(unitData!), savedCharacter.Xp));
        }
    }

    public bool TryGetCharacter(string characterName, out Character character)
    {
        return _characters.TryGetValue(characterName, out character!);
    }
}

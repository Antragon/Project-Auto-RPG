namespace Game.scripts.Units;

using System.Collections.Generic;
using Persistence;

public sealed class CharacterRoster
{
    private readonly Dictionary<string, Unit> _units = [];

    public CharacterRoster(SaveRepository saveRepository)
    {
        foreach (var character in saveRepository.Characters)
        {
            var unitData = UnitDataRepository.Load(character.Name);
            if (unitData is null)
            {
                continue;
            }

            _units.TryAdd(character.Name, new Unit(unitData));
        }
    }

    public bool TryGetUnit(string characterName, out Unit unit)
    {
        return _units.TryGetValue(characterName, out unit!);
    }
}

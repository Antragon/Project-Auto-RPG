namespace Game.scripts.Units;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

public sealed class CharacterRepository
{
    private readonly Dictionary<Guid, Character> _characters;

    private readonly ReadOnlyDictionary<Guid, Character> _readOnlyCharacters;

    public CharacterRepository(Dictionary<Guid, Character> characters)
    {
        _characters = characters;
        _readOnlyCharacters = new ReadOnlyDictionary<Guid, Character>(_characters);
    }

    public IReadOnlyDictionary<Guid, Character> Characters => _readOnlyCharacters;

    public bool TryGetCharacter(Guid characterId, [NotNullWhen(true)] out Character? character)
    {
        return _characters.TryGetValue(characterId, out character);
    }

    public Character GetCharacter(Guid characterId)
    {
        return _characters[characterId];
    }
}

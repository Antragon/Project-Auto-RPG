namespace Game.scripts.Units;

using System.Collections.Generic;

public sealed class CharacterRoster(Dictionary<string, Character> characters)
{
    public IReadOnlyDictionary<string, Character> Characters => field ??= characters.AsReadOnly();

    public bool TryGetCharacter(string characterName, out Character character)
    {
        return characters.TryGetValue(characterName, out character!);
    }
}

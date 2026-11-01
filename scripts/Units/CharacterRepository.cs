namespace Game.scripts.Units;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

public sealed class CharacterRepository : IDisposable
{
    private readonly EnemyDefeatRepository _enemyDefeatRepository;

    private readonly EnemyScalingService _enemyScalingService;

    private readonly Dictionary<Guid, Character> _characters;

    private readonly ReadOnlyDictionary<Guid, Character> _readOnlyCharacters;

    public CharacterRepository(Dictionary<Guid, Character> characters, EnemyDefeatRepository enemyDefeatRepository, EnemyScalingService enemyScalingService)
    {
        _characters = characters;
        _readOnlyCharacters = new ReadOnlyDictionary<Guid, Character>(_characters);
        _enemyDefeatRepository = enemyDefeatRepository;
        _enemyScalingService = enemyScalingService;
        _enemyDefeatRepository.Changed += RefreshEnemyBonuses;
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

    private void RefreshEnemyBonuses()
    {
        foreach (var character in _characters.Values)
        {
            _enemyScalingService.Apply(character.Unit);
        }
    }

    public void Dispose()
    {
        _enemyDefeatRepository.Changed -= RefreshEnemyBonuses;
    }
}

namespace Game.scripts.Units;

using System;
using System.Collections.Generic;

public sealed class CharacterFormationService
{
    public const int SlotCount = 4;

    private readonly List<string?> _characters = [null, null, null, null];

    public event Action? Changed;

    public IReadOnlyList<string?> Characters => field ??= _characters.AsReadOnly();

    public string? GetCharacter(int slotIndex)
    {
        return (uint)slotIndex < _characters.Count ? _characters[slotIndex] : null;
    }

    public bool IsSlotted(string characterName)
    {
        return _characters.Contains(characterName);
    }

    public void Assign(string characterName, int targetSlotIndex)
    {
        if (string.IsNullOrWhiteSpace(characterName) || (uint)targetSlotIndex >= SlotCount)
        {
            return;
        }

        var sourceSlotIndex = _characters.IndexOf(characterName);
        if (sourceSlotIndex == targetSlotIndex)
        {
            return;
        }

        if (sourceSlotIndex >= 0)
        {
            _characters[sourceSlotIndex] = null;
        }

        _characters[targetSlotIndex] = characterName;
        Changed?.Invoke();
    }
}
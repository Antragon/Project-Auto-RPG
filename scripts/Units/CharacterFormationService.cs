namespace Game.scripts.Units;

using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;

public sealed class CharacterFormationService
{
    public const int SlotCount = 4;

    private readonly List<UnitData?> _units = [null, null, null, null];

    public event Action? Changed;

    public IReadOnlyList<UnitData?> Units => field ??= _units.AsReadOnly();

    public UnitData? GetUnitData(int slotIndex)
    {
        return (uint)slotIndex < _units.Count ? _units[slotIndex] : null;
    }

    public bool IsSlotted(string characterName)
    {
        return _units.Any(unitData => unitData?.Name == characterName);
    }

    public void Assign(UnitData unitData, int targetSlotIndex)
    {
        if ((uint)targetSlotIndex >= SlotCount)
        {
            return;
        }

        var sourceSlotIndex = _units.FindIndex(assignedUnitData => assignedUnitData?.Name == unitData.Name);
        if (sourceSlotIndex == targetSlotIndex)
        {
            return;
        }

        if (sourceSlotIndex >= 0)
        {
            _units[sourceSlotIndex] = null;
        }

        _units[targetSlotIndex] = unitData;
        Changed?.Invoke();
    }
}

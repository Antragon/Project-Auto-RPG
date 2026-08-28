namespace Game.scripts.Units;

using System;
using System.Linq;
using Dungeon;
using Extensions;
using Godot;

public partial class CharacterFormationService : Node2D
{
    private UnitSlot CharacterSlot1 => field ??= GetNode<UnitSlot>("CharacterSlot1");

    private UnitSlot CharacterSlot2 => field ??= GetNode<UnitSlot>("CharacterSlot2");

    private UnitSlot CharacterSlot3 => field ??= GetNode<UnitSlot>("CharacterSlot3");

    private UnitSlot CharacterSlot4 => field ??= GetNode<UnitSlot>("CharacterSlot4");

    private UnitSlot[] CharacterSlots => field ??= [CharacterSlot1, CharacterSlot2, CharacterSlot3, CharacterSlot4];

    private bool _reconciling;

    public event Action? Changed;

    public override void _Ready()
    {
        foreach (var characterSlot in CharacterSlots)
        {
            characterSlot.Changed += OnCharacterSlotChanged;
        }
    }

    public override void _ExitTree()
    {
        foreach (var characterSlot in CharacterSlots)
        {
            characterSlot.Changed -= OnCharacterSlotChanged;
        }
    }

    public bool IsSlotted(string characterName)
    {
        return CharacterSlots.Any(slot => slot.Unit?.UnitData.Name == characterName);
    }

    private void OnCharacterSlotChanged(UnitSlot changedSlot)
    {
        if (_reconciling)
        {
            return;
        }

        _reconciling = true;
        try
        {
            var changedUnit = changedSlot.Unit;
            if (changedUnit is not null)
            {
                foreach (var characterSlot in CharacterSlots)
                {
                    if (characterSlot != changedSlot && characterSlot.Unit?.UnitData.Name == changedUnit.UnitData.Name)
                    {
                        characterSlot.Clear();
                    }
                }
            }
        }
        finally
        {
            _reconciling = false;
        }

        Changed?.Invoke();
    }
}

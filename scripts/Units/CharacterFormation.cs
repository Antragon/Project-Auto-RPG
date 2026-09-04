namespace Game.scripts.Units;

using System;
using System.Linq;
using Dungeon;
using Extensions;

public partial class CharacterFormation : UnitFormation
{
    private bool _reconciling;

    private DungeonState _dungeonState;

    public event Action? Changed;

    public bool CanStartEncounter => Slots.Any(slot => slot.Unit is not null)
        && Slots.All(slot => slot.Unit is null || !slot.Unit.IsDead);

    public override void _Ready()
    {
        foreach (var characterSlot in Slots)
        {
            characterSlot.PropertyChanged += OnCharacterSlotPropertyChanged;
        }
    }

    public override void _ExitTree()
    {
        foreach (var characterSlot in Slots)
        {
            characterSlot.PropertyChanged -= OnCharacterSlotPropertyChanged;
        }
    }

    public override void Update(DungeonState dungeonState)
    {
        PushStateToSlots(dungeonState);
        _dungeonState = dungeonState;
    }

    public bool IsSlotted(string characterName)
    {
        return Slots.Any(slot => slot.Unit?.UnitData.Name == characterName);
    }

    private void OnCharacterSlotPropertyChanged(UnitSlot sender, string propertyName)
    {
        if (propertyName != nameof(UnitSlot.Unit))
        {
            return;
        }

        if (_reconciling)
        {
            return;
        }

        _reconciling = true;
        try
        {
            var changedUnit = sender.Unit;
            if (changedUnit is not null)
            {
                foreach (var characterSlot in Slots)
                {
                    if (characterSlot != sender && ReferenceEquals(characterSlot.Unit, changedUnit))
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

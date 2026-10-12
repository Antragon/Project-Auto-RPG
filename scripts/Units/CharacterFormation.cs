namespace Game.scripts.Units;

using System;
using System.Linq;
using Extensions;
using Dungeon;

public partial class CharacterFormation : UnitFormation
{
    private bool _reconciling;

    private DungeonState _dungeonState;

    private CharacterSlot[] CharacterSlots => field ??= this.GetChildrenOfType<CharacterSlot>();

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

    public bool IsSlotted(Unit unit)
    {
        return Slots.Any(slot => slot.Unit == unit);
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
                foreach (var characterSlot in CharacterSlots)
                {
                    if (characterSlot.UnitSlot != sender && ReferenceEquals(characterSlot.UnitSlot.Unit, changedUnit))
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

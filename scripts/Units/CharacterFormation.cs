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
            characterSlot.Changed += OnCharacterSlotChanged;
        }
    }

    public override void _ExitTree()
    {
        foreach (var characterSlot in Slots)
        {
            characterSlot.Changed -= OnCharacterSlotChanged;
        }
    }

    public override void Update(DungeonState dungeonState)
    {
        PushStateToSlots(dungeonState);

        if (_dungeonState == dungeonState)
        {
            return;
        }

        _dungeonState = dungeonState;
        var animationName = dungeonState switch
        {
            DungeonState.Walking => "walk",
            DungeonState.Combat => "idle",
            _ => null,
        };

        if (animationName is null)
        {
            return;
        }

        foreach (var characterSlot in Slots)
        {
            characterSlot.PlayAnimation(animationName);
        }
    }

    public bool IsSlotted(string characterName)
    {
        return Slots.Any(slot => slot.Unit?.UnitData.Name == characterName);
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
                foreach (var characterSlot in Slots)
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

        if (_dungeonState == DungeonState.Walking && changedSlot.Unit is not null)
        {
            changedSlot.PlayAnimation("walk");
        }

        Changed?.Invoke();
    }
}

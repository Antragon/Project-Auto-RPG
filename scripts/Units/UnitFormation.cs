namespace Game.scripts.Units;

using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Dungeon;
using Extensions;
using Godot;

public abstract partial class UnitFormation : Node2D
{
    public ReadOnlyCollection<UnitSlot> Slots => field ??= this.GetChildrenOfType<UnitSlot>().AsReadOnly();

    public bool AllUnitsDead
    {
        get
        {
            var unitSlots = Slots
                .Where(slot => slot.Unit is not null)
                .ToArray();
            return unitSlots.Length != 0 && unitSlots
                .Select(slot => slot.GetChildOfType<UnitAnimation>())
                .All(anim => anim.UnitIsDead);
        }
    }

    public abstract void Update(DungeonState dungeonState);

    protected void PushStateToSlots(DungeonState dungeonState)
    {
        foreach (var slot in Slots)
        {
            slot.SetState(dungeonState);
        }
    }
}

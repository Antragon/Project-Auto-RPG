namespace Game.scripts.Units;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dungeon;
using Extensions;
using Godot;

public abstract partial class UnitFormation: Node2D
{

    public ReadOnlyCollection<UnitSlot> Slots => field ??= this.GetChildrenOfType<UnitSlot>().AsReadOnly();

    public abstract void Update(DungeonState dungeonState);
}

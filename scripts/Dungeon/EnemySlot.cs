namespace Game.scripts.Dungeon;

using Extensions;
using Godot;

public partial class EnemySlot : Node2D
{
    public UnitSlot UnitSlot => field ??= this.GetChildOfType<UnitSlot>();
}

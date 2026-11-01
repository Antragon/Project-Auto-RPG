namespace Game.scripts.Dungeon;

using Extensions;
using Godot;
using Units;

public partial class EnemySlot : Node2D
{
    public UnitSlot UnitSlot => field ??= this.GetChildOfType<UnitSlot>();

    public Enemy? Enemy { get; private set; }

    public void Assign(Enemy enemy)
    {
        Enemy = enemy;
        UnitSlot.Assign(enemy.Unit);
    }

    public void Clear()
    {
        Enemy = null;
        UnitSlot.Clear();
    }
}

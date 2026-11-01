namespace Game.scripts.Dungeon;

using Godot;
using Godot.Collections;
using Units;

[GlobalClass]
public partial class DungeonData : Resource
{
    [Export] public Array<EnemyData> Enemies { get; set; } = [];
}

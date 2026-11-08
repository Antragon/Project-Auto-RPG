namespace Game.scripts.Dungeon;

using Godot;
using Godot.Collections;
using Units;

[GlobalClass]
public partial class DungeonTierData : Resource
{
    [Export] public Array<EnemyData> Enemies { get; set; } = [];

    [Export(PropertyHint.Range, "1,1000,1,or_greater")]
    public int Level { get; set; } = 1;
}

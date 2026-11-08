namespace Game.scripts.Dungeon;

using Godot;
using Godot.Collections;

[GlobalClass]
public partial class DungeonData : Resource
{
    [Export] public Array<DungeonTierData> Tiers { get; set; } = [];
}

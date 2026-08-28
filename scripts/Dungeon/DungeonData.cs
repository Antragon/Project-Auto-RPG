namespace Game.scripts.Dungeon;

using Godot;
using Godot.Collections;

[GlobalClass]
public partial class DungeonData : Resource
{
    [Export] public Array<string> Enemies { get; set; } = [];
}

namespace Game.scripts.Loot;

using Godot;

[GlobalClass]
public partial class LootData : Resource
{
    [Export] public Texture2D? Icon { get; set; }
}

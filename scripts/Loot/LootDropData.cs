namespace Game.scripts.Loot;

using Godot;

[GlobalClass]
public partial class LootDropData : Resource
{
    [Export] public string Name { get; set; } = string.Empty;

    [Export] public int Amount { get; set; } = 1;

    [Export(PropertyHint.Range, "0,100,0.1")] public float Chance { get; set; }
}

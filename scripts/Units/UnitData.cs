namespace Game.scripts.Units;

using Godot;
using Godot.Collections;
using Skills;

[GlobalClass]
public partial class UnitData : Resource
{
    [Export] public Array<string> Tags { get; set; } = [];

    [Export] public SkillTargetData BaseAttack { get; set; } = new();

    [Export] public int Vitality { get; set; }

    [Export] public int Strength { get; set; }

    [Export] public int Dexterity { get; set; }

    [Export] public int Power { get; set; }

    [Export] public int Speed { get; set; }
}

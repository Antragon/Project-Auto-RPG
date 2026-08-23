namespace Game.scripts.Units;

using Godot;
using Godot.Collections;
using Skills;

[GlobalClass]
public partial class UnitData : Resource
{
    [Export] public Array<string> Tags { get; set; } = [];

    [Export] public SkillTargetData BaseAttack { get; set; } = new();

    [Export] public int Hp { get; set; }

    [Export] public int Attack { get; set; }

    [Export] public int Defense { get; set; }

    [Export] public int MagicAttack { get; set; }

    [Export] public int MagicDefense { get; set; }

    [Export] public int Speed { get; set; }
}

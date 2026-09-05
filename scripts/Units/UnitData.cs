namespace Game.scripts.Units;

using Godot;
using Godot.Collections;
using Skills;

[GlobalClass]
public partial class UnitData : Resource
{
    [Export] public Array<string> Tags { get; set; } = [];

    [Export] public SkillData BaseSkill { get; set; } = null!;

    [Export] public int Vitality { get; set; }

    [Export] public int Strength { get; set; }

    [Export] public int Dexterity { get; set; }

    [Export] public int Magic { get; set; }

    [Export] public int Speed { get; set; }
}

namespace Game.scripts.Units;

using Godot;
using Skills;

[GlobalClass]
public partial class EnemyData : Resource
{
    [Export] public SkillData? Skill1 { get; set; }

    [Export] public SkillData? Skill2 { get; set; }

    [Export] public SkillData? Skill3 { get; set; }

    [Export] public SkillData? Skill4 { get; set; }
}

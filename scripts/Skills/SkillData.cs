namespace Game.scripts.Skills;

using Godot;

[GlobalClass]
public partial class SkillData : Resource
{
    [Export] public Godot.Collections.Array<SkillTargetData> Targets { get; set; } = [];
}

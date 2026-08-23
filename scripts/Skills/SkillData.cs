namespace Game.scripts.Skills;

using Godot;

public enum SkillTargetSelection
{
    First,
    Last,
    All,
    Random,
}

public enum SkillTarget
{
    Enemy,
    Character,
}

public enum SkillEffect
{
    Damage,
    Heal,
}

[GlobalClass]
public partial class SkillData : Resource
{
    [Export] public SkillTarget Target { get; set; } = SkillTarget.Enemy;

    [Export] public SkillTargetSelection Selection { get; set; } = SkillTargetSelection.First;

    [Export] public SkillEffect Effect { get; set; } = SkillEffect.Damage;

    [Export] public int Amount { get; set; }
}

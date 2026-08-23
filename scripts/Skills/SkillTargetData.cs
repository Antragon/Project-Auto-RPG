namespace Game.scripts.Skills;

using Godot;

public enum SkillTargetSelection
{
    Specific,
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
public partial class SkillTargetData : Resource
{
    [Export] public SkillTarget Target { get; set; } = SkillTarget.Enemy;

    [Export] public SkillTargetSelection Selection { get; set; } = SkillTargetSelection.Specific;

    [Export(PropertyHint.Range, "1,4,1")] public int Slot { get; set; } = 1;

    [Export] public SkillEffect Effect { get; set; } = SkillEffect.Damage;

    [Export] public int Amount { get; set; }
}

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
    Opponent,
    Ally,
}

public enum SkillEffect
{
    Damage,
    Heal,
}

[GlobalClass]
public partial class SkillData : Resource
{
    [Export] public Texture2D? Icon { get; set; }

    [Export] public float Cooldown { get; set; } = 2f;

    [Export] public SkillTarget Target { get; set; } = SkillTarget.Opponent;

    [Export] public SkillTargetSelection Selection { get; set; } = SkillTargetSelection.First;

    [Export] public SkillEffect Effect { get; set; } = SkillEffect.Damage;

    [Export] public int Power { get; set; }
}

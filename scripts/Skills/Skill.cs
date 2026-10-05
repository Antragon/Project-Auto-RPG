namespace Game.scripts.Skills;

public sealed class Skill
{
    public Skill(SkillData skillData)
    {
        SkillData = skillData;
    }

    public SkillData SkillData { get; }
}

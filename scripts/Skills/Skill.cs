namespace Game.scripts.Skills;

using System;

public sealed class Skill
{
    public Skill(Guid id, SkillData skillData)
    {
        Id = id;
        SkillData = skillData;
    }

    public Guid Id { get; }

    public SkillData SkillData { get; }
}

namespace Game.scripts.Skills;

using System;
using System.Collections.Generic;
using Persistence;

public sealed class SkillRepositoryFactory(SaveRepository saveRepository)
{
    public SkillRepository Create()
    {
        var skills = new Dictionary<Guid, Skill>();
        foreach (var savedSkill in saveRepository.SaveData.Progress.Skills)
        {
            var skillData = SkillDataRepository.Load(savedSkill.Name);
            if (skillData is not null)
            {
                skills.Add(savedSkill.Id, new Skill(savedSkill.Id, skillData));
            }
        }

        return new SkillRepository(skills);
    }
}

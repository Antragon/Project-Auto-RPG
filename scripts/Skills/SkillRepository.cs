namespace Game.scripts.Skills;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Persistence;

public sealed class SkillRepository
{
    private readonly Dictionary<Guid, Skill> _skills = [];

    private readonly ReadOnlyDictionary<Guid, Skill> _readOnlySkills;

    public SkillRepository(SaveRepository saveRepository)
    {
        foreach (var savedSkill in saveRepository.Skills)
        {
            var skillData = SkillDataRepository.Load(savedSkill.Name);
            if (skillData is not null)
            {
                _skills.Add(savedSkill.Id, new Skill(savedSkill.Id, skillData));
            }
        }

        _readOnlySkills = new ReadOnlyDictionary<Guid, Skill>(_skills);
    }

    public IReadOnlyDictionary<Guid, Skill> Skills => _readOnlySkills;

    public bool TryGetSkill(Guid skillId, [NotNullWhen(true)] out Skill? skill)
    {
        return _skills.TryGetValue(skillId, out skill);
    }

    public Skill GetSkill(Guid skillId)
    {
        return _skills[skillId];
    }
}

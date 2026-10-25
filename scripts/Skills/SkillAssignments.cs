namespace Game.scripts.Skills;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

public class SkillAssignments
{
    private readonly Dictionary<Guid, SkillAssignment> _skillAssignments = [];

    private readonly Dictionary<SkillAssignment, Guid> _skillIds = [];

    public void Set(Guid skillId, SkillAssignment skillAssignment)
    {
        if (_skillIds.TryGetValue(skillAssignment, out var previousSkillId))
        {
            _skillAssignments.Remove(previousSkillId);
        }

        if (_skillAssignments.TryGetValue(skillId, out var previousAssignment))
        {
            _skillIds.Remove(previousAssignment);
        }

        _skillAssignments[skillId] = skillAssignment;
        _skillIds[skillAssignment] = skillId;
    }

    public void Remove(SkillAssignment skillAssignment)
    {
        if (_skillIds.TryGetValue(skillAssignment, out var skillId))
        {
            _skillAssignments.Remove(skillId);
            _skillIds.Remove(skillAssignment);
        }
    }

    public bool TryGet(Guid skillId, [NotNullWhen(true)] out SkillAssignment? skillAssignment)
    {
        return _skillAssignments.TryGetValue(skillId, out skillAssignment);
    }

    public SkillAssignment Get(Guid skillId)
    {
        return _skillAssignments[skillId];
    }

    public Guid GetSkillId(SkillAssignment skillAssignment)
    {
        return _skillIds[skillAssignment];
    }
}

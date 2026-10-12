namespace Game.scripts.Skills;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

public class SkillAssignments
{
    private readonly Dictionary<SkillData, SkillAssignment> _skillAssignments = [];

    private readonly Dictionary<SkillAssignment, SkillData> _skillData = [];

    public void Add(SkillData skillData, SkillAssignment skillAssignment)
    {
        _skillAssignments[skillData] = skillAssignment;
        _skillData[skillAssignment] = skillData;
    }

    public void Remove(SkillAssignment skillAssignment)
    {
        if (_skillData.TryGetValue(skillAssignment, out var skillData))
        {
            _skillAssignments.Remove(skillData);
            _skillData.Remove(skillAssignment);
        }
    }

    public bool TryGet(SkillData skillData, [NotNullWhen(true)] out SkillAssignment? skillAssignment)
    {
        return _skillAssignments.TryGetValue(skillData, out skillAssignment);
    }

    public SkillAssignment Get(SkillData skillData)
    {
        return _skillAssignments[skillData];
    }

    public SkillData GetSkillData(SkillAssignment skillAssignment)
    {
        return _skillData[skillAssignment];
    }
}

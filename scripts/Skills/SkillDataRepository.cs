namespace Game.scripts.Skills;

using Godot;
using Units;

public static class SkillDataRepository
{
    public static SkillData? Load(string name)
    {
        var skillData = GD.Load<SkillData>($"res://resources/skills/{name}.tres");
        if (skillData is not null)
        {
            return skillData;
        }

        skillData = GD.Load<UnitData>($"res://resources/units/{name}.tres")?.BaseSkill;
        if (skillData is null)
        {
            GD.PushWarning($"Could not load skill '{name}'.");
        }

        return skillData;
    }
}

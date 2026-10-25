namespace Game.scripts.Dungeon;

using System;
using Skills;
using Units;

public static class DamageCalculator
{
    public static int CalculatePower(Unit sourceUnit, Unit targetUnit, SkillData skillData)
    {
        if (skillData.Effect != SkillEffect.Damage)
        {
            return Math.Max(0, skillData.Power);
        }

        var power = Math.Max(0, skillData.Power + sourceUnit.Power.Value);
        var (scalingStat, defense) = skillData.Scaling switch
        {
            SkillScaling.Strength => (sourceUnit.Strength, targetUnit.Defense),
            SkillScaling.Dexterity => (sourceUnit.Dexterity, targetUnit.Defense),
            SkillScaling.Magic => (sourceUnit.Magic, targetUnit.MagicDefense),
            _ => throw new ArgumentOutOfRangeException(nameof(skillData.Scaling), skillData.Scaling, null),
        };

        var totalPower = (int)Math.Floor(power * Math.Pow(1.1, scalingStat));
        if (totalPower <= 0)
        {
            return 0;
        }

        var totalDefense = Math.Max(0, defense.Value);
        if (totalDefense == 0)
        {
            return totalPower;
        }

        var reductionFraction = 1 - Math.Pow(0.5, totalDefense / (double)totalPower);
        var reduction = (int)Math.Ceiling(totalPower * reductionFraction);
        return totalPower - reduction;
    }
}

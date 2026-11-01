namespace Game.scripts.Units;

using System;
using System.Linq;
using Extensions;
using Godot;

public sealed class EnemyScalingService(EnemyDefeatRepository enemyDefeatRepository)
{
    private EnemyData[] Enemies => field ??= DirAccess.GetResourceFilesRecursivelyAt("res://resources/enemies")
        .Select(GD.Load<EnemyData>)
        .OfType<EnemyData>()
        .ToArray();

    public EnemyScalingProgress GetProgress(EnemyData enemy)
    {
        return EnemyScalingProgress.FromDefeatCount(enemyDefeatRepository.GetCount(enemy.Name));
    }

    public void Apply(Unit unit)
    {
        foreach (var enemy in Enemies)
        {
            var modifier = enemy.ScalingStat switch
            {
                BaseStat.Vitality => unit.VitalityModifier,
                BaseStat.Strength => unit.StrengthModifier,
                BaseStat.Dexterity => unit.DexterityModifier,
                BaseStat.Magic => unit.MagicModifier,
                BaseStat.Speed => unit.SpeedModifier,
                _ => throw new ArgumentOutOfRangeException(nameof(enemy.ScalingStat), enemy.ScalingStat, null),
            };
            var bonus = GetProgress(enemy).Bonus;
            if (bonus == 0)
            {
                modifier.Remove(enemy);
            }
            else
            {
                modifier[enemy] = bonus;
            }
        }
    }
}

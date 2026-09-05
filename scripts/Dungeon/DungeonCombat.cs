namespace Game.scripts.Dungeon;

using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Godot;
using Skills;
using Units;

public partial class DungeonCombat : Node
{
    private CharacterFormation CharacterFormation => field ??= GetParent<Dungeon>().GetChildOfType<CharacterFormation>();

    private EnemyFormation EnemyFormation => field ??= GetParent<Dungeon>().GetChildOfType<EnemyFormation>();

    private RandomNumberGenerator Random => field ??= CreateRandomNumberGenerator();

    private readonly List<(UnitSkillSlots SkillSlots, Action<SkillData> Handler)> _subscriptions = [];

    public override void _Ready()
    {
        Subscribe(CharacterFormation);
        Subscribe(EnemyFormation);
    }

    public override void _ExitTree()
    {
        foreach (var (skillSlots, handler) in _subscriptions)
        {
            skillSlots.SkillTriggered -= handler;
        }

        _subscriptions.Clear();
    }

    private void Subscribe(UnitFormation formation)
    {
        foreach (var unitSlot in formation.Slots)
        {
            var skillSlots = unitSlot.GetChildOfType<UnitSkillSlots>();
            Action<SkillData> handler = skillData => OnSkillTriggered(unitSlot, skillData);
            skillSlots.SkillTriggered += handler;
            _subscriptions.Add((skillSlots, handler));
        }
    }

    private void OnSkillTriggered(UnitSlot sourceSlot, SkillData skillData)
    {
        if (sourceSlot.Unit is null or { IsDead: true })
        {
            return;
        }

        var targetFormation = GetTargetFormation(sourceSlot.GetParent<UnitFormation>(), skillData.Target);
        var power = CalculatePower(sourceSlot.Unit, skillData);

        foreach (var targetSlot in SelectTargets(targetFormation, skillData.Selection))
        {
            ApplyEffect(targetSlot.Unit!, skillData.Effect, power);
        }
    }

    private UnitFormation GetTargetFormation(UnitFormation sourceFormation, SkillTarget target)
    {
        return target switch
        {
            SkillTarget.Ally => sourceFormation,
            SkillTarget.Opponent when ReferenceEquals(sourceFormation, CharacterFormation) => EnemyFormation,
            SkillTarget.Opponent when ReferenceEquals(sourceFormation, EnemyFormation) => CharacterFormation,
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null),
        };
    }

    private UnitSlot[] SelectTargets(UnitFormation formation, SkillTargetSelection selection)
    {
        var livingSlots = formation.Slots
            .Where(slot => slot.Unit is { IsDead: false })
            .ToArray();

        return selection switch
        {
            SkillTargetSelection.First => [.. livingSlots.Take(1)],
            SkillTargetSelection.Last => [.. livingSlots.TakeLast(1)],
            SkillTargetSelection.All => livingSlots,
            SkillTargetSelection.Random => SelectRandomTarget(livingSlots),
            _ => throw new ArgumentOutOfRangeException(nameof(selection), selection, null),
        };
    }

    private UnitSlot[] SelectRandomTarget(UnitSlot[] livingSlots)
    {
        return livingSlots.Length == 0
            ? []
            : [livingSlots[Random.RandiRange(0, livingSlots.Length - 1)]];
    }

    private static void ApplyEffect(Unit unit, SkillEffect effect, int power)
    {
        switch (effect)
        {
            case SkillEffect.Damage:
                unit.SetHp(unit.Hp - Math.Min(power, unit.Hp));
                break;
            case SkillEffect.Heal:
                unit.SetHp(unit.Hp + Math.Min(power, unit.MaxHp - unit.Hp));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(effect), effect, null);
        }
    }

    private static int CalculatePower(Unit sourceUnit, SkillData skillData)
    {
        var power = Math.Max(0, skillData.Power);
        if (skillData.Effect != SkillEffect.Damage)
        {
            return power;
        }

        var scalingStat = skillData.Scaling switch
        {
            SkillScaling.Strength => sourceUnit.UnitData.Strength,
            SkillScaling.Dexterity => sourceUnit.UnitData.Dexterity,
            SkillScaling.Magic => sourceUnit.UnitData.Magic,
            _ => throw new ArgumentOutOfRangeException(nameof(skillData.Scaling), skillData.Scaling, null),
        };

        return (int)Math.Floor(power * Math.Pow(1.1, scalingStat * sourceUnit.Level));
    }

    private static RandomNumberGenerator CreateRandomNumberGenerator()
    {
        var random = new RandomNumberGenerator();
        random.Randomize();
        return random;
    }
}

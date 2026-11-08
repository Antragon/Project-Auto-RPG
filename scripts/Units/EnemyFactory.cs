namespace Game.scripts.Units;

using System;
using System.Linq;
using Dungeon;
using Extensions;
using Godot;
using Skills;

public partial class EnemyFactory : Node
{
    private RandomNumberGenerator Random => field ??= CreateRandomNumberGenerator();

    private Dungeon Dungeon => field ??= GetParent<Dungeon>();

    private EnemyFormation EnemyFormation => field ??= Dungeon.GetChildOfType<EnemyFormation>();

    public override void _Ready()
    {
        Dungeon.StateChanged += OnDungeonStateChanged;
    }

    public override void _ExitTree()
    {
        Dungeon.StateChanged -= OnDungeonStateChanged;
    }

    private void OnDungeonStateChanged(DungeonState state)
    {
        if (state == DungeonState.Idle && Dungeon.PrepareEncounter() is { } tier)
        {
            SpawnEnemy(tier);
        }
    }

    private bool SpawnEnemy(DungeonTierData tier)
    {
        if (tier is not { Enemies.Count: > 0 })
        {
            return false;
        }

        if (EnemyFormation.Slots.Any(slot => slot.Unit is not null))
        {
            return false;
        }

        var availableSlots = EnemyFormation.EnemySlots.ToList();
        if (availableSlots.Count == 0)
        {
            return false;
        }

        do
        {
            var enemyData = tier.Enemies[Random.RandiRange(0, tier.Enemies.Count - 1)];
            if (enemyData is null)
            {
                return false;
            }

            var unitData = UnitDataRepository.Load(enemyData.Name);
            if (unitData is null)
            {
                return false;
            }

            var unit = new Unit(unitData)
            {
                Level = tier.Level,
                Skill1 = CreateSkill(enemyData.Skill1),
                Skill2 = CreateSkill(enemyData.Skill2),
                Skill3 = CreateSkill(enemyData.Skill3),
                Skill4 = CreateSkill(enemyData.Skill4),
            };
            var slotIndex = Random.RandiRange(0, availableSlots.Count - 1);
            availableSlots[slotIndex].Assign(new Enemy(enemyData, unit));
            availableSlots.RemoveAt(slotIndex);
        }
        while (availableSlots.Count > 0 && Random.Randf() < 0.5f);

        return true;
    }

    private static Skill? CreateSkill(SkillData? skillData)
    {
        return skillData is null ? null : new Skill(Guid.NewGuid(), skillData);
    }

    private static RandomNumberGenerator CreateRandomNumberGenerator()
    {
        var random = new RandomNumberGenerator();
        random.Randomize();
        return random;
    }
}

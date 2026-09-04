namespace Game.scripts.Units;

using System.Linq;
using Dungeon;
using Extensions;
using Godot;

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
        if (state == DungeonState.Idle && Dungeon.Data is { } dungeonData)
        {
            SpawnEnemy(dungeonData);
        }
    }

    private bool SpawnEnemy(DungeonData dungeonData)
    {
        if (dungeonData is not { Enemies.Count: > 0 })
        {
            return false;
        }

        if (EnemyFormation.Slots.Any(slot => slot.Unit is not null))
        {
            return false;
        }

        var enemyName = dungeonData.Enemies[Random.RandiRange(0, dungeonData.Enemies.Count - 1)];
        var enemyData = UnitDataRepository.Load(enemyName);
        if (enemyData is null)
        {
            return false;
        }

        EnemyFormation.Slots[0].Assign(new Unit(enemyData));
        return true;
    }

    private static RandomNumberGenerator CreateRandomNumberGenerator()
    {
        var random = new RandomNumberGenerator();
        random.Randomize();
        return random;
    }
}

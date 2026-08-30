namespace Game.scripts.Units;

using System.Linq;
using Dungeon;
using Godot;

public partial class EnemyFormation : UnitFormation
{
    private const float EnemyApproachDistance = 600f;
    private const float WalkSpeed = 200f;

    private RandomNumberGenerator Random => field ??= CreateRandomNumberGenerator();

    private Vector2 _defaultPosition;
    private DungeonState _dungeonState;

    public override void _Ready()
    {
        _defaultPosition = Position;
    }

    public override void _Process(double delta)
    {
        if (_dungeonState == DungeonState.Walking)
        {
            UpdateWalking(delta);
        }
    }

    private void UpdateWalking(double delta)
    {
        var nextX = Mathf.MoveToward(
            Position.X,
            _defaultPosition.X,
            WalkSpeed * (float)delta);

        Position = new Vector2(nextX, _defaultPosition.Y);
        if (!Mathf.IsEqualApprox(nextX, _defaultPosition.X))
        {
            return;
        }

        Position = _defaultPosition;
    }

    public override void Update(DungeonState dungeonState)
    {
        PushStateToSlots(dungeonState);
        _dungeonState = dungeonState;
        if (_dungeonState == DungeonState.Idle)
        {
            Reset();
        }
    }

    private void Reset()
    {
        Position = _defaultPosition + Vector2.Right * EnemyApproachDistance;

        foreach (var enemySlot in Slots)
        {
            enemySlot.Clear();
        }
    }

    public bool SpawnEnemy(DungeonData dungeonData)
    {
        if (dungeonData is not { Enemies.Count: > 0 })
        {
            return false;
        }

        if (Slots.Any(slot => slot.Unit is not null))
        {
            return false;
        }

        var enemyName = dungeonData.Enemies[Random.RandiRange(0, dungeonData.Enemies.Count - 1)];
        var enemyData = UnitDataRepository.Load(enemyName);
        if (enemyData is null)
        {
            GD.PushWarning($"Could not load enemy unit '{enemyName}'.");
            return false;
        }

        Slots[0].Assign(enemyData);
        return true;
    }

    private static RandomNumberGenerator CreateRandomNumberGenerator()
    {
        var random = new RandomNumberGenerator();
        random.Randomize();
        return random;
    }
}

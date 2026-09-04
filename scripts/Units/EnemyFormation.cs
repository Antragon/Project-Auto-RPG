namespace Game.scripts.Units;

using Dungeon;
using Godot;

public partial class EnemyFormation : UnitFormation
{
    private const float EnemyApproachDistance = 600f;
    private const float WalkSpeed = 200f;

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
        _dungeonState = dungeonState;

        if (dungeonState != DungeonState.Walking)
        {
            PushStateToSlots(dungeonState);
        }

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
}

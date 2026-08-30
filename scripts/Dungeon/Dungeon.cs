namespace Game.scripts.Dungeon;

using System;
using Extensions;
using Godot;
using Units;

public partial class Dungeon : Node2D
{
    private const float WalkSpeed = 200f;

    private DungeonBackground Background => field ??= GetNode<DungeonBackground>("Background");

    private CharacterFormation CharacterFormation => field ??= this.GetChildOfType<CharacterFormation>();

    private EnemyFormation EnemyFormation => field ??= this.GetChildOfType<EnemyFormation>();

    private Vector2 _backgroundDefaultPosition;
    private Vector2 _enemyFormationDefaultPosition;
    private DungeonState _dungeonState;
    private DungeonData? _dungeonData;

    public override void _Ready()
    {
        _backgroundDefaultPosition = Background.Position;
        _enemyFormationDefaultPosition = EnemyFormation.Position;
    }

    public override void _Process(double delta)
    {
        var currentState = _dungeonState;
        switch (_dungeonState)
        {
            case DungeonState.Idle:
                UpdateIdle();
                break;
            case DungeonState.Walking:
                UpdateWalking(delta);
                break;
            case DungeonState.Combat:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (currentState != _dungeonState)
        {
            PushStateToFormations();
        }
    }

    public void Enter(string name)
    {
        _dungeonData = DungeonDataRepository.Load(name);
        _dungeonState = DungeonState.Idle;
        Background.Position = _backgroundDefaultPosition;
        PushStateToFormations();
    }

    private void UpdateIdle()
    {
        if (_dungeonData is not null && CharacterFormation.CanStartEncounter)
        {
            EnemyFormation.SpawnEnemy(_dungeonData);
            _dungeonState = DungeonState.Walking;
        }
    }

    private void UpdateWalking(double delta)
    {
        var nextX = Background.Position.X - WalkSpeed * (float)delta;
        var tileWidth = Background.TileSet.TileSize.X * Background.Scale.X;

        if (nextX <= _backgroundDefaultPosition.X - tileWidth)
        {
            nextX += tileWidth;
        }

        Background.Position = new Vector2(nextX, _backgroundDefaultPosition.Y);

        if (Mathf.IsEqualApprox(EnemyFormation.Position.X, _enemyFormationDefaultPosition.X))
        {
            _dungeonState = DungeonState.Combat;
        }
    }

    private void PushStateToFormations()
    {
        CharacterFormation.Update(_dungeonState);
        EnemyFormation.Update(_dungeonState);
    }
}

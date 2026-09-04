namespace Game.scripts.Dungeon;

using System;
using Extensions;
using Godot;
using Units;

public partial class Dungeon : Node2D
{
    private CharacterFormation CharacterFormation => field ??= this.GetChildOfType<CharacterFormation>();

    private EnemyFormation EnemyFormation => field ??= this.GetChildOfType<EnemyFormation>();

    private Vector2 _enemyFormationDefaultPosition;

    public event Action<DungeonState>? StateChanged;

    public DungeonData? Data { get; private set; }

    public DungeonState State { get; private set; }

    public override void _Ready()
    {
        _enemyFormationDefaultPosition = EnemyFormation.Position;
    }

    public override void _Process(double delta)
    {
        var currentState = State;
        switch (State)
        {
            case DungeonState.Idle:
                UpdateIdle();
                break;
            case DungeonState.Walking:
                UpdateWalking();
                break;
            case DungeonState.Combat:
                UpdateCombat();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (currentState != State)
        {
            PushStateToFormations();
            StateChanged?.Invoke(State);
        }
    }

    public void Enter(string name)
    {
        Data = DungeonDataRepository.Load(name);
        State = DungeonState.Idle;
        PushStateToFormations();
        StateChanged?.Invoke(State);
    }

    private void UpdateIdle()
    {
        if (Data is not null && CharacterFormation.CanStartEncounter)
        {
            State = DungeonState.Walking;
        }
    }

    private void UpdateWalking()
    {
        if (Mathf.IsEqualApprox(EnemyFormation.Position.X, _enemyFormationDefaultPosition.X))
        {
            State = DungeonState.Combat;
        }
    }

    private void UpdateCombat()
    {
        if (EnemyFormation.AllUnitsDead || CharacterFormation.AllUnitsDead)
        {
            State = DungeonState.Idle;
        }
    }

    private void PushStateToFormations()
    {
        CharacterFormation.Update(State);
        EnemyFormation.Update(State);
    }
}

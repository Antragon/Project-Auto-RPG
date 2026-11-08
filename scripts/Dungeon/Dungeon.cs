namespace Game.scripts.Dungeon;

using System;
using Extensions;
using Godot;
using Persistence;
using Units;

public partial class Dungeon : Node2D
{
    private SaveDataWriter SaveDataWriter => field ??= this.Root.Get<SaveDataWriter>();

    private SaveRepository SaveRepository => field ??= this.Root.Get<SaveRepository>();

    private CharacterFormation CharacterFormation => field ??= this.GetChildOfType<CharacterFormation>();

    private EnemyFormation EnemyFormation => field ??= this.GetChildOfType<EnemyFormation>();

    private Vector2 _enemyFormationDefaultPosition;

    public event Action<DungeonState>? StateChanged;

    public event Action? TiersChanged;

    public DungeonData? Data { get; private set; }

    public DungeonState State { get; private set; }

    public SavedDungeonProgress? Progress { get; private set; }

    public int SelectedTier => SaveRepository.SaveData.PlayerState.SelectedTier;

    public int EncounterTier { get; private set; } = 1;

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
        var data = DungeonDataRepository.Load(name);
        if (data is null)
        {
            return;
        }

        Data = data;
        var save = SaveRepository.SaveData;
        if (!save.Progress.DungeonProgress.TryGetValue(name, out var progress))
        {
            progress = new SavedDungeonProgress();
            save.Progress.DungeonProgress[name] = progress;
        }

        Progress = progress;
        var selectionChanged = save.PlayerState.EnterDungeon(name, progress, data.Tiers.Count);

        State = DungeonState.Idle;
        PushStateToFormations();
        StateChanged?.Invoke(State);
        TiersChanged?.Invoke();
        if (selectionChanged) SaveDataWriter.Save();
    }

    public void SelectTier(int tier)
    {
        if (Data is null || Progress is null || !Progress.IsUnlocked(tier, Data.Tiers.Count)) return;
        var selectionChanged = SaveRepository.SaveData.PlayerState.SelectTier(tier, Progress, Data.Tiers.Count);
        State = DungeonState.Idle;
        PushStateToFormations();
        StateChanged?.Invoke(State);
        TiersChanged?.Invoke();
        if (selectionChanged) SaveDataWriter.Save();
    }

    public DungeonTierData? PrepareEncounter()
    {
        if (Data is null || Progress is null || !Progress.IsUnlocked(SelectedTier, Data.Tiers.Count))
        {
            return null;
        }

        EncounterTier = SelectedTier;
        return Data.Tiers[EncounterTier - 1];
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
            if (Progress is not null)
            {
                Progress.EncounteredEnemies ??= [];
                var changed = false;
                foreach (var slot in EnemyFormation.EnemySlots)
                {
                    if (slot.Enemy is { } enemy)
                        changed |= Progress.EncounteredEnemies.Add(enemy.EnemyData.Name);
                }

                if (changed) SaveDataWriter.Save();
            }
            State = DungeonState.Combat;
        }
    }

    private void UpdateCombat()
    {
        var encounterWon = EnemyFormation.AllUnitsDead;
        if (encounterWon || CharacterFormation.AllUnitsDead)
        {
            if (encounterWon && Data is not null && Progress is not null)
            {
                Progress.RecordDefeat(EncounterTier, Data.Tiers.Count);
                TiersChanged?.Invoke();
            }

            State = DungeonState.Idle;
            SaveDataWriter.Save();
        }
    }

    private void PushStateToFormations()
    {
        CharacterFormation.Update(State);
        EnemyFormation.Update(State);
    }
}

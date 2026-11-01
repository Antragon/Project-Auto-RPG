namespace Game.scripts.Persistence;

using Extensions;
using Godot;
using Units;

public partial class EnemyDefeatSaver : Node
{
    private EnemyDefeatRepository EnemyDefeatRepository => field ??= this.Root.Get<EnemyDefeatRepository>();

    private SaveDataWriter SaveDataWriter => field ??= this.Root.Get<SaveDataWriter>();

    public override void _Ready()
    {
        EnemyDefeatRepository.Changed += OnEnemyDefeatsChanged;
    }

    public override void _ExitTree()
    {
        EnemyDefeatRepository.Changed -= OnEnemyDefeatsChanged;
    }

    private void OnEnemyDefeatsChanged()
    {
        SaveDataWriter.Save();
    }
}

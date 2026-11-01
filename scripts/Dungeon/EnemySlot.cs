namespace Game.scripts.Dungeon;

using Extensions;
using Godot;
using Units;

public partial class EnemySlot : Node2D
{
    private EnemyDefeatRepository EnemyDefeatRepository => field ??= this.Root.Get<EnemyDefeatRepository>();

    private bool _defeatRecorded;

    public UnitSlot UnitSlot => field ??= this.GetChildOfType<UnitSlot>();

    public Enemy? Enemy { get; private set; }

    public override void _Ready()
    {
        UnitSlot.UnitPropertyChanged += OnUnitPropertyChanged;
    }

    public override void _ExitTree()
    {
        UnitSlot.UnitPropertyChanged -= OnUnitPropertyChanged;
    }

    public void Assign(Enemy enemy)
    {
        Enemy = enemy;
        _defeatRecorded = false;
        UnitSlot.Assign(enemy.Unit);
    }

    public void Clear()
    {
        Enemy = null;
        UnitSlot.Clear();
    }

    private void OnUnitPropertyChanged(UnitSlot sender, string propertyName)
    {
        if (propertyName != nameof(Unit.IsDead) || sender.Unit?.IsDead != true
            || Enemy is null || _defeatRecorded)
        {
            return;
        }

        _defeatRecorded = true;
        EnemyDefeatRepository.RecordDefeat(Enemy.EnemyData.Name);
    }
}

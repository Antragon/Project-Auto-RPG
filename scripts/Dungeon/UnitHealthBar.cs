namespace Game.scripts.Dungeon;

using Godot;

public partial class UnitHealthBar : ProgressBar
{
    private UnitSlot UnitSlot => field ??= GetParent<UnitSlot>();

    public override void _Ready()
    {
        MaxValue = 100;
        UnitSlot.PropertyChanged += OnUnitSlotPropertyChanged;
        UnitSlot.UnitPropertyChanged += OnUnitPropertyChanged;
        Refresh();
    }

    public override void _ExitTree()
    {
        UnitSlot.PropertyChanged -= OnUnitSlotPropertyChanged;
        UnitSlot.UnitPropertyChanged -= OnUnitPropertyChanged;
    }

    private void OnUnitSlotPropertyChanged(UnitSlot sender, string propertyName)
    {
        if (propertyName == nameof(UnitSlot.Unit))
        {
            Refresh();
        }
    }

    private void OnUnitPropertyChanged(UnitSlot sender, string propertyName)
    {
        Refresh();
    }

    private void Refresh()
    {
        var unit = UnitSlot.Unit;
        if (unit is null)
        {
            Visible = false;
            return;
        }

        Value = unit.MaxHp > 0 ? unit.Hp / (double)unit.MaxHp * 100 : 0;
        Visible = true;
    }
}

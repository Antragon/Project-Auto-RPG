namespace Game.scripts.Dungeon;

using Godot;
using Units;

public partial class UnitLevelLabel : Label
{
    private UnitSlot UnitSlot => field ??= GetParent<UnitSlot>();

    public override void _Ready()
    {
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
        if (propertyName == nameof(Unit.Level))
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        var unit = UnitSlot.Unit;
        Visible = unit is not null;
        Text = unit is null ? string.Empty : $"Lv. {unit.Level}";
    }
}

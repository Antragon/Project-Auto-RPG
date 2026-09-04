namespace Game.scripts.Dungeon;

using Godot;
using Units;

public partial class UnitHealthBar : ProgressBar
{
    private UnitSlot UnitSlot => field ??= GetParent<UnitSlot>();

    private Unit? _subscribedUnit;

    public override void _Ready()
    {
        MaxValue = 100;
        UnitSlot.PropertyChanged += OnUnitSlotPropertyChanged;
        SubscribeToUnit(UnitSlot.Unit);
    }

    public override void _ExitTree()
    {
        UnitSlot.PropertyChanged -= OnUnitSlotPropertyChanged;
        SubscribeToUnit(null);
    }

    private void OnUnitSlotPropertyChanged(UnitSlot sender, string propertyName)
    {
        if (propertyName == nameof(UnitSlot.Unit))
        {
            SubscribeToUnit(sender.Unit);
        }
    }

    private void SubscribeToUnit(Unit? unit)
    {
        if (_subscribedUnit is not null)
        {
            _subscribedUnit.HpChanged -= OnUnitHpChanged;
        }

        _subscribedUnit = unit;
        if (_subscribedUnit is not null)
        {
            _subscribedUnit.HpChanged += OnUnitHpChanged;
        }

        Refresh();
    }

    private void OnUnitHpChanged()
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

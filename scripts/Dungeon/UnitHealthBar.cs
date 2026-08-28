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
        UnitSlot.Changed += OnUnitSlotChanged;
        SubscribeToUnit(UnitSlot.Unit);
    }

    public override void _ExitTree()
    {
        UnitSlot.Changed -= OnUnitSlotChanged;
        SubscribeToUnit(null);
    }

    private void OnUnitSlotChanged(UnitSlot changedSlot)
    {
        SubscribeToUnit(changedSlot.Unit);
    }

    private void SubscribeToUnit(Unit? unit)
    {
        if (_subscribedUnit is not null)
        {
            _subscribedUnit.Changed -= OnUnitChanged;
        }

        _subscribedUnit = unit;
        if (_subscribedUnit is not null)
        {
            _subscribedUnit.Changed += OnUnitChanged;
        }

        Refresh();
    }

    private void OnUnitChanged()
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
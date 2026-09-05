namespace Game.scripts.UI;

using Godot;
using Units;

public partial class CharacterInventoryDeadMarker : Node2D
{
    private CharacterInventoryIcon CharacterInventoryIcon => GetParent<CharacterInventoryIcon>();

    private Unit Unit => CharacterInventoryIcon.Character.Unit;

    public override void _Ready()
    {
        Unit.PropertyChanged += OnUnitPropertyChanged;
        Refresh();
    }

    public override void _ExitTree()
    {
        Unit.PropertyChanged -= OnUnitPropertyChanged;
    }

    private void OnUnitPropertyChanged(Unit unit, string propertyName)
    {
        if (propertyName == nameof(Units.Unit.IsDead))
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        Visible = Unit.IsDead;
    }
}

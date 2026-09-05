namespace Game.scripts.UI;

using Godot;
using Units;

public partial class CharacterInventoryLevelLabel : Label
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
        if (propertyName == nameof(Unit.Level))
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        Text = $"Lv. {Unit.Level}";
    }
}

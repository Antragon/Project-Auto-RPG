namespace Game.scripts.UI;

using Godot;
using Units;

public partial class CharacterInventoryDeadMarker : Node2D
{
    private Character? Character { get; set; }

    public override void _ExitTree()
    {
        if (Character is not null)
        {
            Character.Unit.PropertyChanged -= OnUnitPropertyChanged;
        }
    }

    public void SetCharacter(Character character)
    {
        if (Character is not null)
        {
            Character.Unit.PropertyChanged -= OnUnitPropertyChanged;
        }

        Character = character;
        Character.Unit.PropertyChanged += OnUnitPropertyChanged;
        Refresh();
    }

    private void OnUnitPropertyChanged(Unit unit, string propertyName)
    {
        Refresh();
    }

    private void Refresh()
    {
        Visible = Character?.Unit.IsDead == true;
    }
}

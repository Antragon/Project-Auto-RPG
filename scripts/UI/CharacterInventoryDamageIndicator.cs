namespace Game.scripts.UI;

using Godot;
using Units;

public partial class CharacterInventoryDamageIndicator : ProgressBar
{
    private Character? Character { get; set; }

    public override void _Ready()
    {
        MaxValue = 100;
    }

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
        var unit = Character?.Unit;
        Value = unit?.MaxHp > 0
            ? (unit.MaxHp - unit.Hp) / (double)unit.MaxHp * 100
            : 0;
    }
}

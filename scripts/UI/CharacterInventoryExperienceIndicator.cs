namespace Game.scripts.UI;

using Godot;
using Units;

public partial class CharacterInventoryExperienceIndicator : ProgressBar
{
    private CharacterInventoryIcon CharacterInventoryIcon => GetParent<CharacterInventoryIcon>();

    private Character Character => CharacterInventoryIcon.Character;

    public override void _Ready()
    {
        ShowPercentage = false;
        Character.PropertyChanged += OnCharacterPropertyChanged;
        Refresh();
    }

    public override void _ExitTree()
    {
        Character.PropertyChanged -= OnCharacterPropertyChanged;
    }

    private void OnCharacterPropertyChanged(Character character, string propertyName)
    {
        if (propertyName == nameof(Character.Xp))
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        MaxValue = Character.XpToNextLevel;
        Value = Character.XpInCurrentLevel;
    }
}

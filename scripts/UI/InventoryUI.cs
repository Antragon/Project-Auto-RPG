namespace Game.scripts.UI;

using Godot;

public partial class InventoryUI : Panel
{
    private CharacterDetailsPanel CharacterDetailsPanel => field ??= GetNode<CharacterDetailsPanel>("CharacterDetailsPanel");

    private CharactersPanel CharactersPanel => field ??= GetNode<CharactersPanel>("CharactersPanel");

    private CharacterInventoryIcon? _selectedCharacterIcon;

    public override void _Ready()
    {
        CharacterDetailsPanel.Closed += OnCharacterDetailsClosed;
        CloseCharacterDetails();
        foreach (var icon in CharactersPanel.Icons)
        {
            icon.Clicked += OnCharacterIconClicked;
        }
    }

    public override void _ExitTree()
    {
        CharacterDetailsPanel.Closed -= OnCharacterDetailsClosed;
        foreach (var icon in CharactersPanel.Icons)
        {
            icon.Clicked -= OnCharacterIconClicked;
        }
    }

    private void OnCharacterIconClicked(CharacterInventoryIcon icon)
    {
        if (_selectedCharacterIcon == icon)
        {
            CloseCharacterDetails();
            return;
        }

        _selectedCharacterIcon?.SetDetailsSelected(false);
        _selectedCharacterIcon = icon;
        _selectedCharacterIcon.SetDetailsSelected(true);
        CharacterDetailsPanel.ShowCharacter(icon.Character.Id);
    }

    private void CloseCharacterDetails()
    {
        _selectedCharacterIcon?.SetDetailsSelected(false);
        _selectedCharacterIcon = null;
        CharacterDetailsPanel.HideCharacter();
    }

    private void OnCharacterDetailsClosed()
    {
        _selectedCharacterIcon?.SetDetailsSelected(false);
        _selectedCharacterIcon = null;
    }
}

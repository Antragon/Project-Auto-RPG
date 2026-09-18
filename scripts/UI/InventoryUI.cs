namespace Game.scripts.UI;

using Extensions;
using Godot;
using Persistence;
using Units;

public partial class InventoryUI : Panel
{
    private CharacterDetailsPanel CharacterDetailsPanel => field ??= GetNode<CharacterDetailsPanel>("CharacterDetailsPanel");

    private SaveRepository SaveRepository => field ??= this.Root.Get<SaveRepository>();

    private CharacterRoster CharacterRoster => field ??= this.Root.Get<CharacterRoster>();

    private CharacterFormation CharacterFormation => field ??= this.Root.Get<CharacterFormation>();

    private GridContainer CharactersGrid => field ??= GetNode<GridContainer>("CharactersPanel/Scroll/Grid");

    private PackedScene CharacterInventoryIconScene => field ??= SceneRepository.Load("ui/CharacterInventoryIcon")!;

    private CharacterInventoryIcon? _selectedCharacterIcon;

    public override void _Ready()
    {
        CharacterFormation.Changed += RefreshSlottedStates;
        CharacterDetailsPanel.Closed += OnCharacterDetailsClosed;
        PopulateCharacters();
    }

    public override void _ExitTree()
    {
        CharacterFormation.Changed -= RefreshSlottedStates;
        CharacterDetailsPanel.Closed -= OnCharacterDetailsClosed;
    }

    private void PopulateCharacters()
    {
        CloseCharacterDetails();

        foreach (var child in CharactersGrid.GetChildren())
        {
            if (child is CharacterInventoryIcon icon)
            {
                icon.Clicked -= OnCharacterIconClicked;
            }

            child.Free();
        }

        foreach (var character in SaveRepository.Characters)
        {
            if (!CharacterRoster.TryGetCharacter(character.Name, out var playableCharacter))
            {
                continue;
            }

            var icon = CharacterInventoryIconScene.Instantiate<CharacterInventoryIcon>();
            icon.SetCharacter(character.Name, playableCharacter);
            icon.Clicked += OnCharacterIconClicked;
            CharactersGrid.AddChild(icon);
        }

        RefreshSlottedStates();
    }

    private void RefreshSlottedStates()
    {
        foreach (var child in CharactersGrid.GetChildren())
        {
            if (child is CharacterInventoryIcon icon)
            {
                icon.SetSlotted(CharacterFormation.IsSlotted(icon.CharacterName));
            }
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
        CharacterDetailsPanel.ShowCharacter(icon.CharacterName, icon.Character);
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

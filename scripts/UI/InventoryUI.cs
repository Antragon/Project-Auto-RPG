namespace Game.scripts.UI;

using Extensions;
using Godot;
using Persistence;
using Units;

public partial class InventoryUI : Panel
{
    private Panel CharacterDetailsPanel => field ??= GetNode<Panel>("CharacterDetailsPanel");

    private Label CharacterDetailsName => field ??= GetNode<Label>("CharacterDetailsPanel/CharacterName");

    private Button CharacterDetailsCloseButton => field ??= GetNode<Button>("CharacterDetailsPanel/CloseButton");

    private SaveRepository SaveRepository => field ??= this.Root.Get<SaveRepository>();

    private CharacterRoster CharacterRoster => field ??= this.Root.Get<CharacterRoster>();

    private CharacterFormation CharacterFormation => field ??= this.Root.Get<CharacterFormation>();

    private GridContainer CharactersGrid => field ??= GetNode<GridContainer>("CharactersPanel/Scroll/Grid");

    private PackedScene CharacterInventoryIconScene => field ??= SceneRepository.Load("ui/CharacterInventoryIcon")!;

    private CharacterInventoryIcon? _selectedCharacterIcon;

    public override void _Ready()
    {
        CharacterFormation.Changed += RefreshSlottedStates;
        CharacterDetailsCloseButton.Pressed += CloseCharacterDetails;
        PopulateCharacters();
    }

    public override void _ExitTree()
    {
        CharacterFormation.Changed -= RefreshSlottedStates;
        CharacterDetailsCloseButton.Pressed -= CloseCharacterDetails;
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
        CharacterDetailsName.Text = icon.CharacterName;
        CharacterDetailsPanel.Visible = true;
    }

    private void CloseCharacterDetails()
    {
        _selectedCharacterIcon?.SetDetailsSelected(false);
        _selectedCharacterIcon = null;
        CharacterDetailsPanel.Visible = false;
    }
}

namespace Game.scripts.UI;

using Godot;

public partial class InventoryUI : Panel
{
    private CharacterDetailsPanel CharacterDetailsPanel => field ??= GetNode<CharacterDetailsPanel>("CharacterDetailsPanel");

    private CharactersPanel CharactersPanel => field ??= GetNode<CharactersPanel>("CharactersPanel");

    private SkillDetailsPanel SkillDetailsPanel => field ??= GetNode<SkillDetailsPanel>("SkillDetailsPanel");

    private SkillsPanel SkillsPanel => field ??= GetNode<SkillsPanel>("SkillsPanel");

    private CharacterInventoryIcon? _selectedCharacterIcon;

    private SkillInventoryIcon? _selectedSkillIcon;

    public override void _Ready()
    {
        CharacterDetailsPanel.Closed += OnCharacterDetailsClosed;
        SkillDetailsPanel.Closed += OnSkillDetailsClosed;
        CloseCharacterDetails();
        CloseSkillDetails();
        foreach (var icon in CharactersPanel.Icons)
        {
            icon.Clicked += OnCharacterIconClicked;
        }

        foreach (var icon in SkillsPanel.Icons)
        {
            icon.Clicked += OnSkillIconClicked;
        }
    }

    public override void _ExitTree()
    {
        CharacterDetailsPanel.Closed -= OnCharacterDetailsClosed;
        SkillDetailsPanel.Closed -= OnSkillDetailsClosed;
        foreach (var icon in CharactersPanel.Icons)
        {
            icon.Clicked -= OnCharacterIconClicked;
        }

        foreach (var icon in SkillsPanel.Icons)
        {
            icon.Clicked -= OnSkillIconClicked;
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

    private void OnSkillIconClicked(SkillInventoryIcon icon)
    {
        if (_selectedSkillIcon == icon)
        {
            CloseSkillDetails();
            return;
        }

        _selectedSkillIcon?.SetDetailsSelected(false);
        _selectedSkillIcon = icon;
        _selectedSkillIcon.SetDetailsSelected(true);
        SkillDetailsPanel.ShowSkill(icon.Skill);
    }

    private void CloseSkillDetails()
    {
        _selectedSkillIcon?.SetDetailsSelected(false);
        _selectedSkillIcon = null;
        SkillDetailsPanel.HideSkill();
    }

    private void OnSkillDetailsClosed()
    {
        _selectedSkillIcon?.SetDetailsSelected(false);
        _selectedSkillIcon = null;
    }
}

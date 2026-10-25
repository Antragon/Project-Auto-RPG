namespace Game.scripts.UI;

using Godot;

public partial class InventoryUI : Panel
{
    private CharacterDetailsPanel CharacterDetailsPanel => field ??= GetNode<CharacterDetailsPanel>("CharacterDetailsPanel");

    private CharactersPanel CharactersPanel => field ??= GetNode<CharactersPanel>("CharactersPanel");

    private SkillDetailsPanel SkillDetailsPanel => field ??= GetNode<SkillDetailsPanel>("SkillDetailsPanel");

    private EquipmentDetailsPanel EquipmentDetailsPanel => field ??= GetNode<EquipmentDetailsPanel>("EquipmentDetailsPanel");

    private SkillsPanel SkillsPanel => field ??= GetNode<SkillsPanel>("SkillsPanel");

    private EquipmentPanel EquipmentPanel => field ??= GetNode<EquipmentPanel>("EquipmentPanel");

    private CharacterInventoryIcon? _selectedCharacterIcon;

    private SkillInventoryIcon? _selectedSkillIcon;

    private EquipmentInventoryIcon? _selectedEquipmentIcon;

    public override void _Ready()
    {
        CharacterDetailsPanel.Closed += OnCharacterDetailsClosed;
        SkillDetailsPanel.Closed += OnSkillDetailsClosed;
        EquipmentDetailsPanel.Closed += OnEquipmentDetailsClosed;
        CloseCharacterDetails();
        CloseSkillDetails();
        CloseEquipmentDetails();
        foreach (var icon in CharactersPanel.Icons)
        {
            icon.Clicked += OnCharacterIconClicked;
        }

        foreach (var icon in SkillsPanel.Icons)
        {
            icon.Clicked += OnSkillIconClicked;
        }

        foreach (var icon in EquipmentPanel.Icons)
        {
            icon.Clicked += OnEquipmentIconClicked;
        }
    }

    public override void _ExitTree()
    {
        CharacterDetailsPanel.Closed -= OnCharacterDetailsClosed;
        SkillDetailsPanel.Closed -= OnSkillDetailsClosed;
        EquipmentDetailsPanel.Closed -= OnEquipmentDetailsClosed;
        foreach (var icon in CharactersPanel.Icons)
        {
            icon.Clicked -= OnCharacterIconClicked;
        }

        foreach (var icon in SkillsPanel.Icons)
        {
            icon.Clicked -= OnSkillIconClicked;
        }

        foreach (var icon in EquipmentPanel.Icons)
        {
            icon.Clicked -= OnEquipmentIconClicked;
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

    private void OnEquipmentIconClicked(EquipmentInventoryIcon icon)
    {
        if (_selectedEquipmentIcon == icon)
        {
            CloseEquipmentDetails();
            return;
        }

        _selectedEquipmentIcon?.SetDetailsSelected(false);
        _selectedEquipmentIcon = icon;
        _selectedEquipmentIcon.SetDetailsSelected(true);
        EquipmentDetailsPanel.ShowEquipment(icon.Equipment);
    }

    private void CloseEquipmentDetails()
    {
        _selectedEquipmentIcon?.SetDetailsSelected(false);
        _selectedEquipmentIcon = null;
        EquipmentDetailsPanel.HideEquipment();
    }

    private void OnEquipmentDetailsClosed()
    {
        _selectedEquipmentIcon?.SetDetailsSelected(false);
        _selectedEquipmentIcon = null;
    }
}

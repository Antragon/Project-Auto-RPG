namespace Game.scripts.UI;

using System;
using System.Linq;
using Extensions;
using Godot;
using Skills;
using EquipmentInstance = Game.scripts.Equipment.Equipment;

public partial class InventoryUI : Panel
{
    private CharacterDetailsPanel CharacterDetailsPanel => field ??= GetNode<CharacterDetailsPanel>("CharacterDetailsPanel");

    private CharactersPanel CharactersPanel => field ??= GetNode<CharactersPanel>("CharactersPanel");

    private SkillDetailsPanel SkillDetailsPanel => field ??= GetNode<SkillDetailsPanel>("SkillDetailsPanel");

    private EquipmentDetailsPanel EquipmentDetailsPanel => field ??= GetNode<EquipmentDetailsPanel>("EquipmentDetailsPanel");

    private SkillsPanel SkillsPanel => field ??= GetNode<SkillsPanel>("SkillsPanel");

    private EquipmentPanel EquipmentPanel => field ??= GetNode<EquipmentPanel>("EquipmentPanel");

    private InventoryPanelLayout CharactersLayout => field ??= new(CharactersPanel);

    private InventoryPanelLayout SkillsLayout => field ??= new(SkillsPanel);

    private InventoryPanelLayout EquipmentLayout => field ??= new(EquipmentPanel);

    private CharacterInventoryIcon? _selectedCharacterIcon;

    private SkillInventoryIcon? _selectedSkillIcon;

    private SkillData? _selectedSkillData;

    private EquipmentInventoryIcon? _selectedEquipmentIcon;

    private Guid? _selectedEquipmentId;

    public override void _Ready()
    {
        CharacterDetailsPanel.Closed += OnCharacterDetailsClosed;
        CharacterDetailsPanel.SkillClicked += OnCharacterSkillClicked;
        CharacterDetailsPanel.EquipmentClicked += OnCharacterEquipmentClicked;
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
        CharacterDetailsPanel.SkillClicked -= OnCharacterSkillClicked;
        CharacterDetailsPanel.EquipmentClicked -= OnCharacterEquipmentClicked;
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
        CharactersLayout.SetCollapsed(true, icon);
    }

    private void CloseCharacterDetails()
    {
        _selectedCharacterIcon?.SetDetailsSelected(false);
        _selectedCharacterIcon = null;
        CharacterDetailsPanel.HideCharacter();
        CharactersLayout.SetCollapsed(false);
    }

    private void OnCharacterDetailsClosed()
    {
        _selectedCharacterIcon?.SetDetailsSelected(false);
        _selectedCharacterIcon = null;
        CharactersLayout.SetCollapsed(false);
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
        _selectedSkillData = icon.Skill.SkillData;
        CharacterDetailsPanel.SetSelectedSkill(_selectedSkillData);
        SkillDetailsPanel.ShowSkill(icon.Skill);
        SkillsLayout.SetCollapsed(true, icon);
    }

    private void OnCharacterSkillClicked(SkillData skillData)
    {
        if (_selectedSkillData == skillData && SkillDetailsPanel.Visible)
        {
            CloseSkillDetails();
            return;
        }

        _selectedSkillIcon?.SetDetailsSelected(false);
        _selectedSkillIcon = SkillsPanel.Icons.FirstOrDefault(icon => icon.Skill.SkillData == skillData);
        _selectedSkillIcon?.SetDetailsSelected(true);
        _selectedSkillData = skillData;
        CharacterDetailsPanel.SetSelectedSkill(skillData);
        var displayName = skillData.ResourcePath.Contains("::") || string.IsNullOrWhiteSpace(skillData.Name)
            ? "Base attack"
            : skillData.Name;
        SkillDetailsPanel.ShowSkill(skillData, displayName);
        SkillsLayout.SetCollapsed(true, _selectedSkillIcon);
    }

    private void CloseSkillDetails()
    {
        _selectedSkillIcon?.SetDetailsSelected(false);
        _selectedSkillIcon = null;
        _selectedSkillData = null;
        CharacterDetailsPanel.SetSelectedSkill(null);
        SkillDetailsPanel.HideSkill();
        SkillsLayout.SetCollapsed(false);
    }

    private void OnSkillDetailsClosed()
    {
        CloseSkillDetails();
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
        _selectedEquipmentId = icon.Equipment.Id;
        CharacterDetailsPanel.SetSelectedEquipment(_selectedEquipmentId);
        EquipmentDetailsPanel.ShowEquipment(icon.Equipment);
        EquipmentLayout.SetCollapsed(true, icon);
    }

    private void OnCharacterEquipmentClicked(EquipmentInstance equipment)
    {
        if (_selectedEquipmentId == equipment.Id && EquipmentDetailsPanel.Visible)
        {
            CloseEquipmentDetails();
            return;
        }

        _selectedEquipmentIcon?.SetDetailsSelected(false);
        _selectedEquipmentIcon = EquipmentPanel.Icons.FirstOrDefault(icon => icon.Equipment.Id == equipment.Id);
        _selectedEquipmentIcon?.SetDetailsSelected(true);
        _selectedEquipmentId = equipment.Id;
        CharacterDetailsPanel.SetSelectedEquipment(equipment.Id);
        EquipmentDetailsPanel.ShowEquipment(equipment);
        EquipmentLayout.SetCollapsed(true, _selectedEquipmentIcon);
    }

    private void CloseEquipmentDetails()
    {
        _selectedEquipmentIcon?.SetDetailsSelected(false);
        _selectedEquipmentIcon = null;
        _selectedEquipmentId = null;
        CharacterDetailsPanel.SetSelectedEquipment(null);
        EquipmentDetailsPanel.HideEquipment();
        EquipmentLayout.SetCollapsed(false);
    }

    private void OnEquipmentDetailsClosed()
    {
        CloseEquipmentDetails();
    }

    private sealed class InventoryPanelLayout(Panel panel)
    {
        private readonly ScrollContainer _scroll = panel.GetChildOfType<ScrollContainer>();
        private readonly Label _title = panel.GetNode<Label>("Title");
        private readonly float _expandedTop = panel.OffsetTop;
        private float _expandedScrollTop;
        private float _expandedScrollBottom;
        private int _expandedScrollVertical;
        private bool _collapsed;

        public void SetCollapsed(bool collapsed, Control? selectedIcon = null)
        {
            if (_collapsed != collapsed)
            {
                if (collapsed)
                {
                    _expandedScrollTop = _scroll.OffsetTop;
                    _expandedScrollBottom = _scroll.OffsetBottom;
                    _expandedScrollVertical = _scroll.ScrollVertical;
                    var rowHeight = _scroll.GetChildOfType<GridContainer>().GetChildrenOfType<Control>()
                        .Select(icon => icon.GetCombinedMinimumSize().Y)
                        .DefaultIfEmpty(0)
                        .Max();
                    const float padding = 5;
                    panel.OffsetTop = panel.OffsetBottom - rowHeight - padding * 2;
                    _scroll.OffsetTop = padding;
                    _scroll.OffsetBottom = padding + rowHeight;
                }
                else
                {
                    panel.OffsetTop = _expandedTop;
                    _scroll.OffsetTop = _expandedScrollTop;
                    _scroll.OffsetBottom = _expandedScrollBottom;
                    _scroll.SetDeferred(ScrollContainer.PropertyName.ScrollVertical, _expandedScrollVertical);
                }

                _title.Visible = !collapsed;
                _collapsed = collapsed;
            }

            if (collapsed && selectedIcon is not null)
            {
                _scroll.CallDeferred(ScrollContainer.MethodName.EnsureControlVisible, selectedIcon);
            }
        }
    }
}

namespace Game.scripts.UI;

using System.Linq;
using Extensions;
using Godot;

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
        SkillDetailsPanel.ShowSkill(icon.Skill);
        SkillsLayout.SetCollapsed(true, icon);
    }

    private void CloseSkillDetails()
    {
        _selectedSkillIcon?.SetDetailsSelected(false);
        _selectedSkillIcon = null;
        SkillDetailsPanel.HideSkill();
        SkillsLayout.SetCollapsed(false);
    }

    private void OnSkillDetailsClosed()
    {
        _selectedSkillIcon?.SetDetailsSelected(false);
        _selectedSkillIcon = null;
        SkillsLayout.SetCollapsed(false);
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
        EquipmentLayout.SetCollapsed(true, icon);
    }

    private void CloseEquipmentDetails()
    {
        _selectedEquipmentIcon?.SetDetailsSelected(false);
        _selectedEquipmentIcon = null;
        EquipmentDetailsPanel.HideEquipment();
        EquipmentLayout.SetCollapsed(false);
    }

    private void OnEquipmentDetailsClosed()
    {
        _selectedEquipmentIcon?.SetDetailsSelected(false);
        _selectedEquipmentIcon = null;
        EquipmentLayout.SetCollapsed(false);
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
                        .Max(icon => icon.GetCombinedMinimumSize().Y);
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

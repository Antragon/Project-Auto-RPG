namespace Game.scripts.UI;

using System;
using Extensions;
using Godot;
using Units;

public partial class CharacterDetailsPanel : Panel
{
    private Label CharacterName => field ??= GetNode<Label>("CharacterName");

    private Label CharacterLevel => field ??= GetNode<Label>("Level");

    private Button CloseButton => field ??= GetNode<Button>("CloseButton");

    private CharacterDetailsStatsPanel StatsPanel => field ??= GetNode<CharacterDetailsStatsPanel>("StatsPanel");

    private CharacterDetailsSkillsPanel SkillsPanel => field ??= GetNode<CharacterDetailsSkillsPanel>("SkillsPanel");

    private CharacterRepository CharacterRepository => field ??= this.Root.Get<CharacterRepository>();

    private Character? _character;

    public event Action? Closed;

    public override void _Ready()
    {
        CloseButton.Pressed += OnCloseButtonPressed;
    }

    public override void _ExitTree()
    {
        CloseButton.Pressed -= OnCloseButtonPressed;
        UnbindCharacter();
    }

    public void ShowCharacter(Guid characterId)
    {
        UnbindCharacter();

        _character = CharacterRepository.GetCharacter(characterId);
        _character.Unit.PropertyChanged += OnUnitPropertyChanged;
        CharacterName.Text = _character.Unit.UnitData.Name;
        CharacterLevel.Text = $"Lv. {_character.Unit.Level}";
        StatsPanel.SetCharacter(characterId);
        SkillsPanel.SetCharacter(characterId);
        Visible = true;
    }

    public void HideCharacter()
    {
        UnbindCharacter();
        StatsPanel.SetCharacter(null);
        SkillsPanel.SetCharacter(null);
        Visible = false;
    }

    private void OnCloseButtonPressed()
    {
        HideCharacter();
        Closed?.Invoke();
    }

    private void OnUnitPropertyChanged(Unit unit, string propertyName)
    {
        if (propertyName == nameof(Unit.Level))
        {
            CharacterLevel.Text = $"Lv. {unit.Level}";
        }
    }

    private void UnbindCharacter()
    {
        if (_character is null)
        {
            return;
        }

        _character.Unit.PropertyChanged -= OnUnitPropertyChanged;
        _character = null;
    }
}

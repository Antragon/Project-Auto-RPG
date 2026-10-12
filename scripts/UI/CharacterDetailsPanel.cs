namespace Game.scripts.UI;

using System;
using Godot;
using Units;

public partial class CharacterDetailsPanel : Panel
{
    private Label CharacterName => field ??= GetNode<Label>("CharacterName");

    private Label CharacterLevel => field ??= GetNode<Label>("Level");

    private Button CloseButton => field ??= GetNode<Button>("CloseButton");

    private CharacterDetailsStatsPanel StatsPanel => field ??= GetNode<CharacterDetailsStatsPanel>("StatsPanel");

    private CharacterDetailsSkillsPanel SkillsPanel => field ??= GetNode<CharacterDetailsSkillsPanel>("SkillsPanel");

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

    public void ShowCharacter(string characterName, Character character)
    {
        UnbindCharacter();

        _character = character;
        _character.Unit.PropertyChanged += OnUnitPropertyChanged;
        CharacterName.Text = characterName;
        CharacterLevel.Text = $"Lv. {character.Unit.Level}";
        StatsPanel.SetCharacter(character);
        SkillsPanel.SetCharacter(character);
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

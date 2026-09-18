namespace Game.scripts.UI;

using System;
using Godot;
using Units;

public partial class CharacterDetailsPanel : Panel
{
    private Label CharacterName => field ??= GetNode<Label>("CharacterName");

    private Label CharacterLevel => field ??= GetNode<Label>("Level");

    private Label TagsValue => field ??= GetNode<Label>("StatsPanel/StatsGrid/Tags/Value");

    private Label HpValue => field ??= GetNode<Label>("StatsPanel/StatsGrid/Hp/Value");

    private Label ExperienceValue => field ??= GetNode<Label>("StatsPanel/StatsGrid/Experience/Value");

    private Label VitalityValue => field ??= GetNode<Label>("StatsPanel/StatsGrid/Vitality/Value");

    private Label StrengthValue => field ??= GetNode<Label>("StatsPanel/StatsGrid/Strength/Value");

    private Label DexterityValue => field ??= GetNode<Label>("StatsPanel/StatsGrid/Dexterity/Value");

    private Label MagicValue => field ??= GetNode<Label>("StatsPanel/StatsGrid/Magic/Value");

    private Label SpeedValue => field ??= GetNode<Label>("StatsPanel/StatsGrid/Speed/Value");

    private Button CloseButton => field ??= GetNode<Button>("CloseButton");

    private Character? _character;

    public event Action? Closed;

    public override void _Ready()
    {
        CloseButton.Pressed += OnCloseButtonPressed;
    }

    public override void _ExitTree()
    {
        CloseButton.Pressed -= OnCloseButtonPressed;
        if (_character is not null)
        {
            _character.PropertyChanged -= OnCharacterPropertyChanged;
            _character.Unit.PropertyChanged -= OnUnitPropertyChanged;
        }
    }

    public void ShowCharacter(string characterName, Character character)
    {
        if (_character is not null)
        {
            _character.PropertyChanged -= OnCharacterPropertyChanged;
            _character.Unit.PropertyChanged -= OnUnitPropertyChanged;
        }

        _character = character;
        _character.PropertyChanged += OnCharacterPropertyChanged;
        _character.Unit.PropertyChanged += OnUnitPropertyChanged;
        CharacterName.Text = characterName;
        RefreshStats();
        Visible = true;
    }

    public void HideCharacter()
    {
        if (_character is not null)
        {
            _character.PropertyChanged -= OnCharacterPropertyChanged;
            _character.Unit.PropertyChanged -= OnUnitPropertyChanged;
            _character = null;
        }

        Visible = false;
    }

    private void OnCloseButtonPressed()
    {
        HideCharacter();
        Closed?.Invoke();
    }

    private void OnUnitPropertyChanged(Unit unit, string propertyName)
    {
        if (propertyName is nameof(Unit.Hp) or nameof(Unit.MaxHp) or nameof(Unit.Level))
        {
            RefreshStats();
        }
    }

    private void OnCharacterPropertyChanged(Character character, string propertyName)
    {
        if (propertyName == nameof(Character.Xp))
        {
            RefreshStats();
        }
    }

    private void RefreshStats()
    {
        if (_character is not { } character)
        {
            return;
        }

        var unit = character.Unit;
        var unitData = unit.UnitData;
        CharacterLevel.Text = $"Lv. {unit.Level}";
        TagsValue.Text = $"Tags: {string.Join(", ", unitData.Tags)}";
        HpValue.Text = $"HP: {unit.Hp} / {unit.MaxHp}";
        ExperienceValue.Text = $"XP: {character.XpInCurrentLevel} / {character.XpToNextLevel}";
        VitalityValue.Text = $"Vitality: {unit.Vitality}";
        StrengthValue.Text = $"Strength: {unit.Strength}";
        DexterityValue.Text = $"Dexterity: {unit.Dexterity}";
        MagicValue.Text = $"Magic: {unit.Magic}";
        SpeedValue.Text = $"Speed: {unit.Speed}";
    }
}

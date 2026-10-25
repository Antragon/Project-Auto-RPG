namespace Game.scripts.UI;

using System;
using Extensions;
using Godot;
using Units;

public partial class CharacterDetailsStatsPanel : Panel
{
    private Label TagsValue => field ??= GetNode<Label>("StatsGrid/Tags/Value");

    private Label HpValue => field ??= GetNode<Label>("StatsGrid/Hp/Value");

    private Label ExperienceValue => field ??= GetNode<Label>("StatsGrid/Experience/Value");

    private Label VitalityValue => field ??= GetNode<Label>("StatsGrid/Vitality/Value");

    private Label StrengthValue => field ??= GetNode<Label>("StatsGrid/Strength/Value");

    private Label DexterityValue => field ??= GetNode<Label>("StatsGrid/Dexterity/Value");

    private Label MagicValue => field ??= GetNode<Label>("StatsGrid/Magic/Value");

    private Label SpeedValue => field ??= GetNode<Label>("StatsGrid/Speed/Value");

    private CharacterRepository CharacterRepository => field ??= this.Root.Get<CharacterRepository>();

    private Character? _character;

    public override void _ExitTree()
    {
        UnbindCharacter();
    }

    public void SetCharacter(Guid? characterId)
    {
        UnbindCharacter();

        if (characterId is not { } id || !CharacterRepository.TryGetCharacter(id, out _character))
        {
            _character = null;
            return;
        }

        _character.PropertyChanged += OnCharacterPropertyChanged;
        _character.Unit.PropertyChanged += OnUnitPropertyChanged;
        RefreshStats();
    }

    private void OnCharacterPropertyChanged(Character character, string propertyName)
    {
        if (propertyName == nameof(Character.Xp))
        {
            RefreshStats();
        }
    }

    private void OnUnitPropertyChanged(Unit unit, string propertyName)
    {
        if (propertyName is nameof(Unit.Hp) or nameof(Unit.MaxHp) or nameof(Unit.Level))
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
        TagsValue.Text = $"Tags: {string.Join(", ", unitData.Tags)}";
        HpValue.Text = $"HP: {unit.Hp} / {unit.MaxHp}";
        ExperienceValue.Text = $"XP: {character.XpInCurrentLevel} / {character.XpToNextLevel}";
        VitalityValue.Text = $"Vitality: {unit.Vitality}";
        StrengthValue.Text = $"Strength: {unit.Strength}";
        DexterityValue.Text = $"Dexterity: {unit.Dexterity}";
        MagicValue.Text = $"Magic: {unit.Magic}";
        SpeedValue.Text = $"Speed: {unit.Speed}";
    }

    private void UnbindCharacter()
    {
        if (_character is null)
        {
            return;
        }

        _character.PropertyChanged -= OnCharacterPropertyChanged;
        _character.Unit.PropertyChanged -= OnUnitPropertyChanged;
        _character = null;
    }
}

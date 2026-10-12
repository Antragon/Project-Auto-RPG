namespace Game.scripts.UI;

using System;
using Godot;
using Skills;
using Units;

public partial class CharacterDetailsSkillsPanel : Panel
{
    private CharacterDetailsSkillSlot[] SkillSlots => field ??= [
        GetNode<CharacterDetailsSkillSlot>("Content/SkillGrid/SkillSlot1"),
        GetNode<CharacterDetailsSkillSlot>("Content/SkillGrid/SkillSlot2"),
        GetNode<CharacterDetailsSkillSlot>("Content/SkillGrid/SkillSlot3"),
        GetNode<CharacterDetailsSkillSlot>("Content/SkillGrid/SkillSlot4"),
    ];

    private static readonly SkillSlotBinding[] SkillSlotBindings = [
        new(unit => unit.Skill1, (unit, skill) => unit.Skill1 = skill),
        new(unit => unit.Skill2, (unit, skill) => unit.Skill2 = skill),
        new(unit => unit.Skill3, (unit, skill) => unit.Skill3 = skill),
        new(unit => unit.Skill4, (unit, skill) => unit.Skill4 = skill),
    ];

    private Character? _character;

    public override void _Ready()
    {
        foreach (var skillSlot in SkillSlots)
        {
            skillSlot.SkillDropped += OnSkillDropped;
            skillSlot.UnslotRequested += OnSkillUnslotRequested;
        }
    }

    public override void _ExitTree()
    {
        foreach (var skillSlot in SkillSlots)
        {
            skillSlot.SkillDropped -= OnSkillDropped;
            skillSlot.UnslotRequested -= OnSkillUnslotRequested;
        }

        UnbindCharacter();
    }

    public void SetCharacter(Character? character)
    {
        UnbindCharacter();
        _character = character;

        if (_character is null)
        {
            return;
        }

        _character.Unit.PropertyChanged += OnUnitPropertyChanged;
        RefreshSkills();
    }

    private void OnUnitPropertyChanged(Unit unit, string propertyName)
    {
        if (propertyName is nameof(Unit.Skill1) or nameof(Unit.Skill2) or nameof(Unit.Skill3) or nameof(Unit.Skill4))
        {
            RefreshSkills();
        }
    }

    private void RefreshSkills()
    {
        if (_character is not { } character)
        {
            return;
        }

        var unit = character.Unit;
        foreach (var skillSlot in SkillSlots)
        {
            var skill = SkillSlotBindings[skillSlot.SlotIndex].Get(unit);
            var displayedSkill = skillSlot.SlotIndex == 0
                ? skill ?? unit.UnitData.BaseSkill
                : skill;
            skillSlot.SetSkill(displayedSkill, skill is not null);
        }
    }

    private void OnSkillDropped(int slotIndex, SkillData skillData)
    {
        SetSkill(slotIndex, skillData);
    }

    private void OnSkillUnslotRequested(int slotIndex)
    {
        SetSkill(slotIndex, null);
    }

    private void SetSkill(int slotIndex, SkillData? skillData)
    {
        if (_character is { } character)
        {
            SkillSlotBindings[slotIndex].Set(character.Unit, skillData);
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

    private sealed record SkillSlotBinding(
        Func<Unit, SkillData?> Get,
        Action<Unit, SkillData?> Set);
}

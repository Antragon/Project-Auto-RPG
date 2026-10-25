namespace Game.scripts.UI;

using System;
using Extensions;
using Godot;
using Skills;
using Units;

public partial class CharacterDetailsSkillsPanel : Panel
{
    private CharacterDetailsSkillSlot[] SkillSlots => field ??=
    [
        GetNode<CharacterDetailsSkillSlot>("Content/SkillGrid/SkillSlot1"),
        GetNode<CharacterDetailsSkillSlot>("Content/SkillGrid/SkillSlot2"),
        GetNode<CharacterDetailsSkillSlot>("Content/SkillGrid/SkillSlot3"),
        GetNode<CharacterDetailsSkillSlot>("Content/SkillGrid/SkillSlot4"),
    ];

    private SkillAssignments SkillAssignments =>
        field ??= this.Root.Get<SkillAssignments>();

    private CharacterRepository CharacterRepository =>
        field ??= this.Root.Get<CharacterRepository>();

    private SkillRepository SkillRepository =>
        field ??= this.Root.Get<SkillRepository>();

    private static readonly SkillSlotBinding[] SkillSlotBindings =
    [
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

    public void SetCharacter(Guid? characterId)
    {
        UnbindCharacter();

        if (characterId is not { } id || !CharacterRepository.TryGetCharacter(id, out _character))
        {
            _character = null;
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
            var displayedSkillData = skill?.SkillData
                ?? (skillSlot.SlotIndex == 0 ? unit.UnitData.BaseSkill : null);
            skillSlot.SetSkill(displayedSkillData, skill is not null);
        }
    }

    private void OnSkillDropped(int slotIndex, Guid skillId)
    {
        if (_character is not { } character)
        {
            return;
        }

        SetSkill(character.Unit, slotIndex, skillId);
    }

    private void OnSkillUnslotRequested(int slotIndex)
    {
        if (_character is { } character)
        {
            SetSkill(character.Unit, slotIndex, null);
        }
    }

    private void SetSkill(Unit unit, int slotIndex, Guid? skillId)
    {
        var binding = SkillSlotBindings[slotIndex];
        var currentSkill = binding.Get(unit);
        Skill? skill = null;
        if (skillId is { } id)
        {
            if (!SkillRepository.TryGetSkill(id, out skill))
            {
                return;
            }
        }

        if (currentSkill?.Id == skill?.Id)
        {
            return;
        }

        if (skill is not null && SkillAssignments.TryGet(skill.Id, out var previousAssignment))
        {
            if (CharacterRepository.TryGetCharacter(previousAssignment.CharacterId, out var previousCharacter))
            {
                SkillSlotBindings[previousAssignment.SkillSlotIndex].Set(previousCharacter.Unit, null);
            }
        }

        binding.Set(unit, skill);
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
        Func<Unit, Skill?> Get,
        Action<Unit, Skill?> Set);
}

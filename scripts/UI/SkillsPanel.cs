namespace Game.scripts.UI;

using System;
using System.Collections.Generic;
using Extensions;
using Godot;
using Skills;
using Units;

public partial class SkillsPanel : Panel
{
    private GridContainer SkillsGrid => field ??= GetNode<GridContainer>("Scroll/Grid");

    private CharacterRepository CharacterRepository => field ??= this.Root.Get<CharacterRepository>();

    private SkillRepository SkillRepository => field ??= this.Root.Get<SkillRepository>();

    private SkillAssignments SkillAssignments =>
        field ??= this.Root.Get<SkillAssignments>();

    private PackedScene SkillInventoryIconScene => field ??= SceneRepository.Load("ui/SkillInventoryIcon")!;

    private readonly Dictionary<SkillSlotProperty, Guid> _skillsByProperty = [];

    public SkillInventoryIcon[] Icons => SkillsGrid.GetChildrenOfType<SkillInventoryIcon>();

    public override void _Ready()
    {
        PopulateSkills();
        foreach (var character in CharacterRepository.Characters.Values)
        {
            IndexCharacterSkills(character.Unit);
            character.Unit.PropertyChanged += OnUnitPropertyChanged;
        }
    }

    public override void _ExitTree()
    {
        foreach (var character in CharacterRepository.Characters.Values)
        {
            character.Unit.PropertyChanged -= OnUnitPropertyChanged;
        }
    }

    private void PopulateSkills()
    {
        foreach (var child in SkillsGrid.GetChildren())
        {
            child.QueueFree();
        }

        foreach (var skill in SkillRepository.Skills.Values)
        {
            var icon = SkillInventoryIconScene.Instantiate<SkillInventoryIcon>();
            icon.SetSkill(skill);
            SkillsGrid.AddChild(icon);
        }
    }

    private void OnUnitPropertyChanged(Unit unit, string propertyName)
    {
        if (propertyName is not (nameof(Unit.Skill1) or nameof(Unit.Skill2) or nameof(Unit.Skill3) or nameof(Unit.Skill4)))
        {
            return;
        }

        UpdateSkillAssignment(unit, propertyName, GetSkill(unit, propertyName));
    }

    private void IndexCharacterSkills(Unit unit)
    {
        IndexSkill(unit, nameof(Unit.Skill1), unit.Skill1);
        IndexSkill(unit, nameof(Unit.Skill2), unit.Skill2);
        IndexSkill(unit, nameof(Unit.Skill3), unit.Skill3);
        IndexSkill(unit, nameof(Unit.Skill4), unit.Skill4);
    }

    private void IndexSkill(Unit unit, string propertyName, Skill? skill)
    {
        if (skill is not null)
        {
            _skillsByProperty[new SkillSlotProperty(unit, propertyName)] = skill.Id;
            SetSkillSlotted(skill.Id);
        }
    }

    private void UpdateSkillAssignment(Unit unit, string propertyName, Skill? skill)
    {
        if (_skillsByProperty.Remove(new SkillSlotProperty(unit, propertyName), out var previousSkillId))
        {
            SetSkillSlotted(previousSkillId);
        }

        if (skill is null)
        {
            return;
        }

        _skillsByProperty[new SkillSlotProperty(unit, propertyName)] = skill.Id;
        SetSkillSlotted(skill.Id);
    }

    private void SetSkillSlotted(Guid skillId)
    {
        foreach (var icon in Icons)
        {
            if (icon.Skill.Id == skillId)
            {
                icon.SetSlotted(SkillAssignments.TryGet(skillId, out _));
            }
        }
    }

    private static Skill? GetSkill(Unit unit, string propertyName)
    {
        return propertyName switch
        {
            nameof(Unit.Skill1) => unit.Skill1,
            nameof(Unit.Skill2) => unit.Skill2,
            nameof(Unit.Skill3) => unit.Skill3,
            nameof(Unit.Skill4) => unit.Skill4,
            _ => null,
        };
    }

    private sealed record SkillSlotProperty(Unit Unit, string PropertyName);
}

namespace Game.scripts.UI;

using System.Collections.Generic;
using Extensions;
using Godot;
using Persistence;
using Skills;
using Units;

public partial class SkillsPanel : Panel
{
    private GridContainer SkillsGrid => field ??= GetNode<GridContainer>("Scroll/Grid");

    private SaveRepository SaveRepository => field ??= this.Root.Get<SaveRepository>();

    private CharacterRoster CharacterRoster => field ??= this.Root.Get<CharacterRoster>();

    private SkillAssignments SkillAssignments =>
        field ??= this.Root.Get<SkillAssignments>();

    private PackedScene SkillInventoryIconScene => field ??= SceneRepository.Load("ui/SkillInventoryIcon")!;

    private readonly Dictionary<SkillSlotProperty, SkillData> _skillsByProperty = [];

    public SkillInventoryIcon[] Icons => SkillsGrid.GetChildrenOfType<SkillInventoryIcon>();

    public override void _Ready()
    {
        PopulateSkills();
        foreach (var character in CharacterRoster.Characters.Values)
        {
            IndexCharacterSkills(character.Unit);
            character.Unit.PropertyChanged += OnUnitPropertyChanged;
        }
    }

    public override void _ExitTree()
    {
        foreach (var character in CharacterRoster.Characters.Values)
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

        foreach (var savedSkill in SaveRepository.Skills)
        {
            var skillData = SkillDataRepository.Load(savedSkill.Name);
            if (skillData is null)
            {
                continue;
            }

            var icon = SkillInventoryIconScene.Instantiate<SkillInventoryIcon>();
            icon.SetSkill(new Skill(skillData));
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

    private void IndexSkill(Unit unit, string propertyName, SkillData? skillData)
    {
        if (skillData is not null)
        {
            _skillsByProperty[new SkillSlotProperty(unit, propertyName)] = skillData;
            SetSkillSlotted(skillData);
        }
    }

    private void UpdateSkillAssignment(Unit unit, string propertyName, SkillData? skillData)
    {
        if (_skillsByProperty.Remove(new SkillSlotProperty(unit, propertyName), out var previousSkill))
        {
            SetSkillSlotted(previousSkill);
        }

        if (skillData is null)
        {
            return;
        }

        _skillsByProperty[new SkillSlotProperty(unit, propertyName)] = skillData;
        SetSkillSlotted(skillData);
    }

    private void SetSkillSlotted(SkillData skillData)
    {
        foreach (var icon in Icons)
        {
            if (ReferenceEquals(icon.Skill.SkillData, skillData))
            {
                icon.SetSlotted(SkillAssignments.TryGet(skillData, out _));
            }
        }
    }

    private static SkillData? GetSkill(Unit unit, string propertyName)
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

namespace Game.scripts.UI;

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

    public SkillInventoryIcon[] Icons => SkillsGrid.GetChildrenOfType<SkillInventoryIcon>();

    public override void _Ready()
    {
        PopulateSkills();
        foreach (var character in CharacterRepository.Characters.Values)
        {
            character.Unit.PropertyChanged += OnUnitPropertyChanged;
        }

        RefreshSkillSlots();
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

        RefreshSkillSlots();
    }

    private void RefreshSkillSlots()
    {
        foreach (var icon in Icons)
        {
            icon.SetSlotted(SkillAssignments.TryGet(icon.Skill.Id, out _));
        }
    }
}

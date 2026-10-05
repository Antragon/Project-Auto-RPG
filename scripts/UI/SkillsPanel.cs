namespace Game.scripts.UI;

using Extensions;
using Godot;
using Persistence;
using Skills;

public partial class SkillsPanel : Panel
{
    private GridContainer SkillsGrid => field ??= GetNode<GridContainer>("Scroll/Grid");

    private SaveRepository SaveRepository => field ??= this.Root.Get<SaveRepository>();

    private PackedScene SkillInventoryIconScene => field ??= SceneRepository.Load("ui/SkillInventoryIcon")!;

    public SkillInventoryIcon[] Icons => SkillsGrid.GetChildrenOfType<SkillInventoryIcon>();

    public override void _Ready()
    {
        PopulateSkills();
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
}

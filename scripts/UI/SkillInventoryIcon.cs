namespace Game.scripts.UI;

using Godot;
using Skills;

public partial class SkillInventoryIcon : Panel
{
    private TextureRect Icon => field ??= GetNode<TextureRect>("Icon");

    private PackedScene SkillInventoryIconScene => field ??= SceneRepository.Load("ui/SkillInventoryIcon")!;

    public Skill Skill { get; private set; } = null!;

    public void SetSkill(Skill skill)
    {
        Skill = skill;
        Icon.Texture = skill.SkillData.Icon;
    }

    public override Variant _GetDragData(Vector2 atPosition)
    {
        var preview = SkillInventoryIconScene.Instantiate<SkillInventoryIcon>();
        preview.SetSkill(Skill);
        preview.MouseFilter = MouseFilterEnum.Ignore;
        preview.Size = Size;
        SetDragPreview(preview);

        return Skill.SkillData;
    }
}

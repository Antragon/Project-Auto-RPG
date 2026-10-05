namespace Game.scripts.UI;

using Godot;
using Skills;

public partial class SkillInventoryIcon : Panel
{
    private TextureRect Icon => field ??= GetNode<TextureRect>("Icon");

    public Skill Skill { get; private set; } = null!;

    public void SetSkill(Skill skill)
    {
        Skill = skill;
        Icon.Texture = skill.SkillData.Icon;
    }
}

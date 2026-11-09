namespace Game.scripts.UI;

using System.Linq;
using Extensions;
using Godot;
using Skills;
using Units;
using static CompendiumUIHelpers;

public partial class CompendiumEnemySkills : GridContainer
{
    private PackedScene SkillDetailsPanelScene => field ??= SceneRepository.Load("ui/SkillDetailsPanel")!;

    public void ShowEnemy(EnemyData enemy, UnitData? unitData, int count)
    {
        ClearChildren(this);
        if (count < 30)
        {
            AddChild(CreateLabel("?"));
        }
        else
        {
            SkillData?[] skills = [enemy.Skill1 ?? unitData?.BaseSkill, enemy.Skill2, enemy.Skill3, enemy.Skill4];
            foreach (var skill in skills.OfType<SkillData>())
                AddSkill(skill, skill == unitData?.BaseSkill);
            if (GetChildCount() == 0) AddChild(CreateLabel("No skills available"));
        }

        if (GetChildCount() % 2 != 0)
        {
            AddChild(new Control
            {
                SizeFlagsHorizontal = SizeFlags.ExpandFill,
                MouseFilter = MouseFilterEnum.Ignore,
            });
        }
    }

    private void AddSkill(SkillData skill, bool baseSkill)
    {
        var row = new HBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        row.AddThemeConstantOverride("separation", 12);
        row.AddChild(new TextureRect
        {
            Texture = skill.Icon,
            CustomMinimumSize = new Vector2(48, 48),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            SizeFlagsVertical = SizeFlags.ShrinkBegin,
            MouseFilter = MouseFilterEnum.Ignore,
        });
        var name = skill.ResourcePath.Contains("::") || string.IsNullOrWhiteSpace(skill.Name)
            ? (baseSkill ? "Base attack" : "Skill")
            : skill.Name;
        var panel = SkillDetailsPanelScene.Instantiate<SkillDetailsPanel>();
        panel.ShowCloseButton = false;
        panel.IsEnemySkill = true;
        panel.CustomMinimumSize = new Vector2(304, 170);
        panel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        row.AddChild(panel);
        AddChild(row);
        panel.ShowSkill(skill, name);
    }
}

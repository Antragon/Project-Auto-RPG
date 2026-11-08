namespace Game.scripts.Units;

using Godot;
using Loot;
using Skills;

[GlobalClass]
public partial class EnemyData : Resource
{
    [Export] public BaseStat ScalingStat { get; set; } = BaseStat.Vitality;

    [Export] public int BaseXp { get; set; }

    [Export] public LootData? Loot { get; set; }

    [Export] public SkillData? Skill1 { get; set; }

    [Export] public SkillData? Skill2 { get; set; }

    [Export] public SkillData? Skill3 { get; set; }

    [Export] public SkillData? Skill4 { get; set; }
}

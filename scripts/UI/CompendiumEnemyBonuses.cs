namespace Game.scripts.UI;

using Extensions;
using Godot;
using Units;

public partial class CompendiumEnemyBonuses : VBoxContainer
{
    private EnemyScalingService EnemyScalingService => field ??= this.Root.Get<EnemyScalingService>();

    private Label DefeatCount => field ??= GetNode<Label>("DefeatCount");

    private Label ScalingBonus => field ??= GetNode<Label>("ScalingBonus");

    private HBoxContainer Loot => field ??= GetNode<HBoxContainer>("Loot");

    private TextureRect LootIcon => field ??= Loot.GetChildOfType<TextureRect>();

    private Label LootName => field ??= Loot.GetNode<Label>("Name");

    private HBoxContainer Milestones => field ??= GetNode<HBoxContainer>("Milestones");

    private Label NextBonus => field ??= Milestones.GetNodeOrNull<Label>("Next") ?? GetNode<Label>("Next");

    private Label LastMilestone => field ??= Milestones.GetNode<Label>("LastMilestone");

    private Label TargetMilestone => field ??= Milestones.GetNode<Label>("TargetMilestone");

    private ProgressBar MilestoneProgress => field ??= Milestones.GetChildOfType<ProgressBar>();

    public override void _Ready()
    {
        if (NextBonus.GetParent() != Milestones)
        {
            NextBonus.Reparent(Milestones);
            Milestones.MoveChild(NextBonus, 0);
        }
    }

    public void ShowEnemy(EnemyData enemy, bool encountered, int count)
    {
        var progress = EnemyScalingService.GetProgress(enemy);
        DefeatCount.Text = encountered ? $"Defeated: {count}" : "Defeated: ?";
        ScalingBonus.Text = count >= 10 ? $"All: +{progress.Bonus} {enemy.ScalingStat}" : "All: ?";
        LootIcon.Texture = count >= 10 ? enemy.Loot?.Icon : null;
        LootIcon.Visible = count >= 10 && enemy.Loot is not null;
        LootName.Text = count < 10 ? "?" : enemy.Loot is { } loot ? loot.Name : "None";
        LastMilestone.Visible = count >= 10;
        TargetMilestone.Visible = count >= 10;
        MilestoneProgress.Visible = count >= 10;
        if (count < 10)
        {
            NextBonus.Text = "Next: ?";
            return;
        }

        var last = progress.LastThreshold;
        LastMilestone.Text = last.ToString();
        if (progress.NextThreshold is { } next)
        {
            NextBonus.Text = "Next:";
            TargetMilestone.Text = next.ToString();
            MilestoneProgress.Value = (double)(count - last) / (next - last);
        }
        else
        {
            NextBonus.Text = "Next: Complete";
            TargetMilestone.Text = "\u2014";
            MilestoneProgress.Value = 1;
        }
    }
}

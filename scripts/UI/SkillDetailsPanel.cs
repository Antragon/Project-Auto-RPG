namespace Game.scripts.UI;

using System;
using Extensions;
using Godot;
using Skills;
using Units;

public partial class SkillDetailsPanel : Panel
{
    private Label SkillName => field ??= GetNode<Label>("SkillName");

    private TextureRect ScalingIcon => field ??= GetNode<TextureRect>("Details/PowerRow/ScalingIcon");

    private Label PowerValue => field ??= GetNode<Label>("Details/PowerRow/PowerValue");

    private Label CooldownValue => field ??= GetNode<Label>("Details/CooldownRow/CooldownValue");

    private SkillTargetIndicator[] CharacterTargetSlots => field ??= GetNode<HBoxContainer>("Details/TargetRow/Characters").GetChildrenOfType<SkillTargetIndicator>();

    private SkillTargetIndicator[] EnemyTargetSlots => field ??= GetNode<HBoxContainer>("Details/TargetRow/Enemies").GetChildrenOfType<SkillTargetIndicator>();

    private Button CloseButton => field ??= GetNode<Button>("CloseButton");

    public event Action? Closed;

    public override void _Ready()
    {
        CloseButton.Pressed += OnCloseButtonPressed;
    }

    public override void _ExitTree()
    {
        CloseButton.Pressed -= OnCloseButtonPressed;
    }

    public void ShowSkill(Skill skill)
    {
        var skillData = skill.SkillData;
        SkillName.Text = skillData.Name;
        CooldownValue.Text = $"{skillData.Cooldown:0.##} s";
        PowerValue.Text = skillData.Power.ToString();
        ScalingIcon.Texture = IconRepository.GetUI(GetScalingIconName(skillData.Scaling));
        ScalingIcon.TooltipText = skillData.Scaling.ToString();
        RefreshTargetSlots(skillData);
        Visible = true;
    }

    public void HideSkill()
    {
        Visible = false;
    }

    private void OnCloseButtonPressed()
    {
        HideSkill();
        Closed?.Invoke();
    }

    private void RefreshTargetSlots(SkillData skillData)
    {
        if (skillData.Target == SkillTarget.Ally)
        {
            RefreshTargetSlots(CharacterTargetSlots, skillData);
            ClearTargetSlots(EnemyTargetSlots);
        }
        else
        {
            RefreshTargetSlots(EnemyTargetSlots, skillData);
            ClearTargetSlots(CharacterTargetSlots);
        }
    }

    private static void RefreshTargetSlots(SkillTargetIndicator[] slots, SkillData skillData)
    {
        for (var index = 0; index < slots.Length; index++)
        {
            if (IsTargetedSlot(index, skillData.Selection))
            {
                slots[index].SetEffect(skillData.Effect, skillData.Selection == SkillTargetSelection.Random);
            }
            else
            {
                slots[index].Clear();
            }
        }
    }

    private static void ClearTargetSlots(SkillTargetIndicator[] slots)
    {
        foreach (var slot in slots)
        {
            slot.Clear();
        }
    }

    private static bool IsTargetedSlot(int index, SkillTargetSelection selection)
    {
        return selection switch
        {
            SkillTargetSelection.First => index == 0,
            SkillTargetSelection.Last => index == 3,
            SkillTargetSelection.All or SkillTargetSelection.Random => true,
            _ => false,
        };
    }

    private static string GetScalingIconName(SkillScaling scaling)
    {
        return scaling switch
        {
            SkillScaling.Strength => "strength",
            SkillScaling.Dexterity => "dexterity",
            SkillScaling.Magic => "magic",
            _ => string.Empty,
        };
    }
}

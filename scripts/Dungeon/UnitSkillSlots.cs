namespace Game.scripts.Dungeon;

using System;
using Extensions;
using Godot;

public partial class UnitSkillSlots : HBoxContainer
{
    private UnitSlot UnitSlot => field ??= GetParent<UnitSlot>();

    private SkillSlot[] Slots => field ??= this.GetChildrenOfType<SkillSlot>();

    public event Action? SkillTriggered;

    public override void _Ready()
    {
        foreach (var slot in Slots)
        {
            slot.SkillTriggered += OnSkillTriggered;
        }

        UnitSlot.UnitChanged += Refresh;
        UnitSlot.StateChanged += OnStateChanged;
        Refresh(UnitSlot);
        OnStateChanged();
    }

    public override void _ExitTree()
    {
        foreach (var skillSlot in Slots)
        {
            skillSlot.SkillTriggered -= OnSkillTriggered;
        }

        UnitSlot.UnitChanged -= Refresh;
        UnitSlot.StateChanged -= OnStateChanged;
    }

    private void OnSkillTriggered()
    {
        SkillTriggered?.Invoke();
    }

    private void OnStateChanged()
    {
        foreach (var slot in Slots)
        {
            slot.SetActive(UnitSlot.DungeonState == DungeonState.Combat);
        }
    }

    private void Refresh(UnitSlot changedSlot)
    {
        var unit = changedSlot.Unit;
        Visible = unit is not null;
        Slots[0].SetSkill(unit?.UnitData.BaseSkill);
    }
}

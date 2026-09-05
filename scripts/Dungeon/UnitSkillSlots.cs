namespace Game.scripts.Dungeon;

using System;
using Extensions;
using Godot;
using Skills;
using Units;

public partial class UnitSkillSlots : HBoxContainer
{
    private UnitSlot UnitSlot => field ??= GetParent<UnitSlot>();

    private SkillSlot[] Slots => field ??= this.GetChildrenOfType<SkillSlot>();

    public event Action<SkillData>? SkillTriggered;

    public override void _Ready()
    {
        foreach (var slot in Slots)
        {
            slot.SkillTriggered += OnSkillTriggered;
        }

        UnitSlot.PropertyChanged += OnUnitSlotPropertyChanged;
        UnitSlot.UnitPropertyChanged += OnUnitPropertyChanged;
        Refresh(UnitSlot);
        OnStateChanged();
    }

    public override void _ExitTree()
    {
        foreach (var skillSlot in Slots)
        {
            skillSlot.SkillTriggered -= OnSkillTriggered;
        }

        UnitSlot.PropertyChanged -= OnUnitSlotPropertyChanged;
        UnitSlot.UnitPropertyChanged -= OnUnitPropertyChanged;
    }

    private void OnSkillTriggered(SkillData skillData)
    {
        SkillTriggered?.Invoke(skillData);
    }

    private void OnUnitSlotPropertyChanged(UnitSlot sender, string propertyName)
    {
        if (propertyName == nameof(UnitSlot.Unit))
        {
            Refresh(sender);
        }

        if (propertyName == nameof(UnitSlot.DungeonState))
        {
            OnStateChanged();
        }
    }

    private void OnUnitPropertyChanged(UnitSlot sender, string propertyName)
    {
        OnStateChanged();
    }

    private void OnStateChanged()
    {
        var active = UnitSlot is { DungeonState: DungeonState.Combat, Unit.IsDead: false };

        foreach (var slot in Slots)
        {
            slot.SetActive(active);
        }
    }

    private void Refresh(UnitSlot changedSlot)
    {
        var unit = changedSlot.Unit;
        Visible = unit is not null;
        Slots[0].SetSkill(unit?.UnitData.BaseSkill);
    }
}

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
        Refresh();
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
            Refresh();
            OnStateChanged();
        }

        var skillSlotIndex = propertyName switch
        {
            nameof(Unit.Skill1) => 0,
            nameof(Unit.Skill2) => 1,
            nameof(Unit.Skill3) => 2,
            nameof(Unit.Skill4) => 3,
            _ => -1,
        };

        if (skillSlotIndex >= 0)
        {
            RefreshSkillSlot(skillSlotIndex);
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

    private void Refresh()
    {
        Visible = UnitSlot.Unit is not null;

        for (var i = 0; i < Slots.Length; i++)
        {
            RefreshSkillSlot(i);
        }
    }

    private void RefreshSkillSlot(int index)
    {
        var unit = UnitSlot.Unit;
        var skill = index switch
        {
            0 => unit?.Skill1 ?? unit?.UnitData.BaseSkill,
            1 => unit?.Skill2,
            2 => unit?.Skill3,
            3 => unit?.Skill4,
            _ => null,
        };
        Slots[index].SetSkill(skill);
    }
}

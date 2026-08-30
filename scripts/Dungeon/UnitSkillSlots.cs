namespace Game.scripts.Dungeon;

using Godot;
using Units;

public partial class UnitSkillSlots : HBoxContainer
{
    private UnitSlot UnitSlot => field ??= GetParent<UnitSlot>();

    private SkillSlot BaseSkillSlot => field ??= GetNode<SkillSlot>("Slot1");

    public override void _Ready()
    {
        UnitSlot.Changed += Refresh;
        Refresh(UnitSlot);
    }

    public override void _ExitTree()
    {
        UnitSlot.Changed -= Refresh;
    }

    public void SetActive(bool active)
    {
        BaseSkillSlot.SetActive(active);
    }

    private void Refresh(UnitSlot changedSlot)
    {
        var unit = changedSlot.Unit;
        Visible = unit is not null;
        BaseSkillSlot.SetSkill(unit?.UnitData.BaseSkill);
    }
}

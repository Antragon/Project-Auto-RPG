namespace Game.scripts.Dungeon;

using Godot;

public partial class UnitSkillSlots : HBoxContainer
{
    private UnitSlot UnitSlot => field ??= GetParent<UnitSlot>();

    private SkillSlot BaseSkillSlot => field ??= GetNode<SkillSlot>("Slot1");

    public override void _Ready()
    {
        UnitSlot.UnitChanged += Refresh;
        UnitSlot.StateChanged += OnStateChanged;
        Refresh(UnitSlot);
        OnStateChanged();
    }

    public override void _ExitTree()
    {
        UnitSlot.UnitChanged -= Refresh;
        UnitSlot.StateChanged -= OnStateChanged;
    }

    private void OnStateChanged()
    {
        BaseSkillSlot.SetActive(UnitSlot.DungeonState == DungeonState.Combat);
    }

    private void Refresh(UnitSlot changedSlot)
    {
        var unit = changedSlot.Unit;
        Visible = unit is not null;
        BaseSkillSlot.SetSkill(unit?.UnitData.BaseSkill);
    }
}

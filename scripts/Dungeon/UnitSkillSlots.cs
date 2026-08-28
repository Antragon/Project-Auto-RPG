namespace Game.scripts.Dungeon;

using Godot;

public partial class UnitSkillSlots : HBoxContainer
{
    private UnitSlot UnitSlot => field ??= GetParent<UnitSlot>();

    private TextureRect BaseSkillIcon => field ??= GetNode<TextureRect>("Slot1/Icon");

    public override void _Ready()
    {
        UnitSlot.Changed += Refresh;
        Refresh(UnitSlot);
    }

    public override void _ExitTree()
    {
        UnitSlot.Changed -= Refresh;
    }

    private void Refresh(UnitSlot changedSlot)
    {
        var unit = changedSlot.Unit;
        Visible = unit is not null;
        BaseSkillIcon.Texture = unit?.UnitData.BaseSkill.Icon;
    }
}

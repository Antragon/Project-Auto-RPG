namespace Game.scripts.Dungeon;

using Godot;

public partial class UnitSlotDropTarget : Control
{
    private UnitSlot UnitSlot => field ??= GetParent<UnitSlot>();

    public override void _Notification(int what)
    {
        if (what == NotificationDragEnd)
        {
            UnitSlot.SetDropIndicatorVisible(false);
        }
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        return UnitSlot.CanAcceptCharacter(data);
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        UnitSlot.Assign(data);
        GetTree().CallGroup("character_drop_indicators", nameof(UnitSlot.SetDropIndicatorVisible), false);
    }
}

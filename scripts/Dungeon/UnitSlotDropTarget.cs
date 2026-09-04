namespace Game.scripts.Dungeon;

using Extensions;
using Godot;
using Units;

public partial class UnitSlotDropTarget : Control
{
    private UnitSlot UnitSlot => field ??= GetParent<UnitSlot>();

    private CharacterRoster CharacterRoster => field ??= this.Root.Get<CharacterRoster>();

    public override void _Notification(int what)
    {
        if (what == NotificationDragEnd)
        {
            UnitSlot.SetDropIndicatorVisible(false);
        }
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        return TryGetUnit(data, out var unit) && UnitSlot.CanAcceptCharacter(unit);
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        if (TryGetUnit(data, out var unit))
        {
            UnitSlot.Assign(unit);
        }

        GetTree().CallGroup("character_drop_indicators", nameof(UnitSlot.SetDropIndicatorVisible), false);
    }

    private bool TryGetUnit(Variant data, out Unit unit)
    {
        unit = null!;
        return data.VariantType == Variant.Type.String
            && CharacterRoster.TryGetUnit(data.AsString(), out unit);
    }
}

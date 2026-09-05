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
        return TryGetCharacter(data, out var character) && UnitSlot.CanAcceptCharacter(character.Unit);
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        if (TryGetCharacter(data, out var character))
        {
            UnitSlot.Assign(character.Unit);
        }

        GetTree().CallGroup("character_drop_indicators", nameof(UnitSlot.SetDropIndicatorVisible), false);
    }

    private bool TryGetCharacter(Variant data, out Character character)
    {
        character = null!;
        return data.VariantType == Variant.Type.String
            && CharacterRoster.TryGetCharacter(data.AsString(), out character);
    }
}

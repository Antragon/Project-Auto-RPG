namespace Game.scripts.Dungeon;

using System;
using Extensions;
using Godot;
using Units;

public partial class CharacterSlotDropTarget : Control
{
    private CharacterSlot CharacterSlot => field ??= GetParent<CharacterSlot>();

    private CharacterRepository CharacterRepository => field ??= this.Root.Get<CharacterRepository>();

    public override void _Notification(int what)
    {
        if (what == NotificationDragEnd)
        {
            CharacterSlot.SetDropIndicatorVisible(false);
        }
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        return TryGetCharacter(data, out var character) && CanAccept(character.Unit);
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        if (TryGetCharacter(data, out var character) && CanAccept(character.Unit))
        {
            CharacterSlot.Assign(character);
        }

        GetTree().CallGroup(CharacterSlot.DropIndicatorGroup, nameof(CharacterSlot.SetDropIndicatorVisible), false);
    }

    private bool TryGetCharacter(Variant data, out Character character)
    {
        character = null!;
        if (data.VariantType == Variant.Type.String
            && Guid.TryParse(data.AsString(), out var characterId)
            && CharacterRepository.TryGetCharacter(characterId, out var resolvedCharacter))
        {
            character = resolvedCharacter;
            return true;
        }

        return false;
    }

    private static bool CanAccept(Unit unit)
    {
        return !unit.IsDead;
    }
}

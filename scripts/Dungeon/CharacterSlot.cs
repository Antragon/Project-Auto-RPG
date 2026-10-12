namespace Game.scripts.Dungeon;

using Godot;
using Units;

public partial class CharacterSlot : Node2D
{
    public const string DropIndicatorGroup = "character_drop_indicators";

    private Sprite2D DropIndicatorSprite => field ??= GetNode<Sprite2D>("DropIndicatorSprite");

    public UnitSlot UnitSlot => field ??= GetNode<UnitSlot>("UnitSlot");

    public Character? Character { get; private set; }

    [Export] public int SlotIndex { get; set; }

    public override void _Ready()
    {
        UnitSlot.SlotIndex = SlotIndex;
        AddToGroup(DropIndicatorGroup);
    }

    public void SetDropIndicatorVisible(bool visible)
    {
        DropIndicatorSprite.Visible = visible;
    }

    public void Assign(Character character)
    {
        Character = character;
        UnitSlot.Assign(character.Unit);
    }

    public void Clear()
    {
        Character = null;
        UnitSlot.Clear();
    }
}

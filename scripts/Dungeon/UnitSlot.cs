namespace Game.scripts.Dungeon;

using Godot;
using Units;

public partial class UnitSlot : Node2D
{
    private Sprite2D DropIndicatorSprite => field ??= GetNode<Sprite2D>("DropIndicatorSprite");

    [Export] public int SlotIndex { get; set; }

    [Export] public bool AcceptsCharacters { get; set; }

    public Unit? Unit { get; private set; }

    public DungeonState DungeonState { get; private set; }

    public event PropertyChangedEventHandler<UnitSlot>? PropertyChanged;

    public override void _Ready()
    {
        if (AcceptsCharacters)
        {
            AddToGroup("character_drop_indicators");
        }
    }

    public void SetState(DungeonState dungeonState)
    {
        if (DungeonState == dungeonState)
        {
            return;
        }

        DungeonState = dungeonState;
        PropertyChanged?.Invoke(this, nameof(DungeonState));
    }

    public bool CanAcceptCharacter(Unit unit)
    {
        return AcceptsCharacters;
    }

    public void SetDropIndicatorVisible(bool visible)
    {
        if (AcceptsCharacters)
        {
            DropIndicatorSprite.Visible = visible;
        }
    }

    public void Assign(Unit unit)
    {
        Unit = unit;
        PropertyChanged?.Invoke(this, nameof(Unit));
    }

    public void Clear()
    {
        if (Unit is null)
        {
            return;
        }

        Unit = null;
        PropertyChanged?.Invoke(this, nameof(Unit));
    }

}

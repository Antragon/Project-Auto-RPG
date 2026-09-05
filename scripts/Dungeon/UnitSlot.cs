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

    public event PropertyChangedEventHandler<UnitSlot>? UnitPropertyChanged;

    public override void _Ready()
    {
        if (AcceptsCharacters)
        {
            AddToGroup("character_drop_indicators");
        }

        Unit?.HpChanged += OnUnitHpChanged;
    }

    public override void _ExitTree()
    {
        Unit?.HpChanged -= OnUnitHpChanged;
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
        Unit?.HpChanged -= OnUnitHpChanged;
        Unit = unit;
        Unit.HpChanged += OnUnitHpChanged;
        PropertyChanged?.Invoke(this, nameof(Unit));
    }

    public void Clear()
    {
        if (Unit is null)
        {
            return;
        }

        Unit.HpChanged -= OnUnitHpChanged;
        Unit = null;
        PropertyChanged?.Invoke(this, nameof(Unit));
    }

    private void OnUnitHpChanged()
    {
        if (Unit is null)
        {
            return;
        }

        UnitPropertyChanged?.Invoke(this, nameof(Unit.Hp));

        if (Unit.IsDead)
        {
            UnitPropertyChanged?.Invoke(this, nameof(Unit.IsDead));
        }
    }
}

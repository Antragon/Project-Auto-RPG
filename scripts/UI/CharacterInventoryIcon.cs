namespace Game.scripts.UI;

using Godot;
using Units;

public partial class CharacterInventoryIcon : Panel
{
    private ProgressBar Damage => field ??= GetNode<ProgressBar>("Damage");

    private TextureRect Icon => field ??= GetNode<TextureRect>("Icon");

    private Panel Tick => field ??= GetNode<Panel>("Tick");

    private Node2D DeadMarker => field ??= GetNode<Node2D>("DeadMarker");

    public string CharacterName { get; private set; } = string.Empty;

    private Character? Character { get; set; }

    public override void _Ready()
    {
        Damage.MaxValue = 100;
    }

    public override void _ExitTree()
    {
        if (Character is not null)
        {
            Character.Unit.HpChanged -= OnUnitHpChanged;
        }
    }

    public void SetCharacter(string name, Character character)
    {
        if (Character is not null)
        {
            Character.Unit.HpChanged -= OnUnitHpChanged;
        }

        CharacterName = name;
        Character = character;
        Character.Unit.HpChanged += OnUnitHpChanged;
        Icon.Texture = IconRepository.GetUnit(name);
        Refresh();
    }

    public void SetSlotted(bool slotted)
    {
        Tick.Visible = slotted;
    }

    public override Variant _GetDragData(Vector2 atPosition)
    {
        if (string.IsNullOrWhiteSpace(CharacterName))
        {
            return default;
        }

        GetTree().CallGroup("character_drop_indicators", "SetDropIndicatorVisible", true);

        if (Duplicate() is CharacterInventoryIcon preview)
        {
            preview.MouseFilter = MouseFilterEnum.Ignore;
            preview.SetSlotted(false);
            preview.Size = Size;
            SetDragPreview(preview);
        }

        if (Character is not null)
        {
            return CharacterName;
        }

        return default;
    }

    private void OnUnitHpChanged()
    {
        Refresh();
    }

    private void Refresh()
    {
        var unit = Character?.Unit;
        if (unit is null)
        {
            Damage.Value = 0;
            DeadMarker.Visible = false;
            return;
        }

        Damage.Value = unit.MaxHp > 0
            ? (unit.MaxHp - unit.Hp) / (double)unit.MaxHp * 100
            : 0;
        DeadMarker.Visible = unit.IsDead;
    }
}

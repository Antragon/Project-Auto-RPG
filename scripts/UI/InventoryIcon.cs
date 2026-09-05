namespace Game.scripts.UI;

using Godot;
using Units;

public partial class InventoryIcon : Panel
{
    private TextureRect Icon => field ??= GetNode<TextureRect>("Icon");

    private Panel Tick => field ??= GetNode<Panel>("Tick");

    public string CharacterName { get; private set; } = string.Empty;

    private Character? Character { get; set; }

    public void SetCharacter(string name, Character character)
    {
        CharacterName = name;
        Character = character;
        Icon.Texture = IconRepository.GetUnit(name);
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

        if (Duplicate() is InventoryIcon preview)
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
}

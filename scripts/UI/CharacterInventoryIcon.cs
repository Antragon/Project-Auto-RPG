namespace Game.scripts.UI;

using Godot;
using Units;

public partial class CharacterInventoryIcon : Panel
{
    private TextureRect Icon => field ??= GetNode<TextureRect>("Icon");

    private Panel Tick => field ??= GetNode<Panel>("Tick");

    private PackedScene DragIconScene => field ??= SceneRepository.Load("ui/DragIcon")!;

    public string CharacterName { get; private set; } = string.Empty;

    public Character Character { get; private set; } = null!;

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
        GetTree().CallGroup("character_drop_indicators", "SetDropIndicatorVisible", true);

        var preview = DragIconScene.Instantiate<Panel>();
        preview.MouseFilter = MouseFilterEnum.Ignore;

        var previewIcon = preview.GetNode<TextureRect>("Icon");
        previewIcon.Texture = Icon.Texture;
        previewIcon.Material = Icon.Material;
        preview.Size = Size;
        SetDragPreview(preview);

        return CharacterName;
    }
}

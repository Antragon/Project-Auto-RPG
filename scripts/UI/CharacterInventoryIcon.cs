namespace Game.scripts.UI;

using System;
using Godot;
using Units;

public partial class CharacterInventoryIcon : Panel
{
    private bool _mouseButtonPressed;

    private bool _wasDragged;

    private TextureRect Icon => field ??= GetNode<TextureRect>("Icon");

    private Panel Tick => field ??= GetNode<Panel>("Tick");

    private Panel SelectionBorder => field ??= GetNode<Panel>("SelectionBorder");

    private PackedScene DragIconScene => field ??= SceneRepository.Load("ui/DragIcon")!;

    public string CharacterName { get; private set; } = string.Empty;

    public Character Character { get; private set; } = null!;

    public event Action<CharacterInventoryIcon>? Clicked;

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

    public void SetDetailsSelected(bool selected)
    {
        SelectionBorder.Visible = selected;
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton { ButtonIndex: MouseButton.Left } mouseButton)
        {
            return;
        }

        if (mouseButton.Pressed)
        {
            _mouseButtonPressed = true;
            _wasDragged = false;
            return;
        }

        if (_mouseButtonPressed)
        {
            _mouseButtonPressed = false;
            if (!_wasDragged)
            {
                Clicked?.Invoke(this);
            }

            _wasDragged = false;
        }
    }

    public override Variant _GetDragData(Vector2 atPosition)
    {
        _wasDragged = true;
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

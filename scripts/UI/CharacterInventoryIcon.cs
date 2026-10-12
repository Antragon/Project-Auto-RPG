namespace Game.scripts.UI;

using System;
using Dungeon;
using Extensions;
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

    public Character Character { get; private set; } = null!;

    public event Action<CharacterInventoryIcon>? Clicked;

    public override void _Ready()
    {
        RefreshIcon();
    }

    public void SetCharacter(Character character)
    {
        Character = character;
        if (IsInsideTree())
        {
            RefreshIcon();
        }
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
        GetTree().CallGroup(
            CharacterSlot.DropIndicatorGroup,
            nameof(CharacterSlot.SetDropIndicatorVisible),
            true);

        var preview = DragIconScene.Instantiate<Panel>();
        preview.MouseFilter = MouseFilterEnum.Ignore;

        var previewIcon = preview.GetNode<TextureRect>("Icon");
        previewIcon.Texture = Icon.Texture;
        previewIcon.Material = Icon.Material;
        preview.Size = Size;
        SetDragPreview(preview);

        return Character.Id.ToString();
    }

    private void RefreshIcon()
    {
        Icon.Texture = IconRepository.GetUnit(Character.Unit.UnitData.Name);
    }
}

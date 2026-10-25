namespace Game.scripts.UI;

using System;
using Extensions;
using Equipment;
using Godot;

public partial class EquipmentInventoryIcon : Panel
{
    private bool _mouseButtonPressed;

    private bool _wasDragged;

    private TextureRect Icon => field ??= GetNode<TextureRect>("Icon");

    private Panel Tick => field ??= GetNode<Panel>("Tick");

    private Panel SelectionBorder => field ??= GetNode<Panel>("SelectionBorder");

    private PackedScene EquipmentInventoryIconScene => field ??= SceneRepository.Load("ui/EquipmentInventoryIcon")!;

    public Equipment Equipment { get; private set; } = null!;

    public event Action<EquipmentInventoryIcon>? Clicked;

    public void SetEquipment(Equipment equipment)
    {
        Equipment = equipment;
        Icon.Texture = equipment.EquipmentData.Icon;
        TooltipText = equipment.EquipmentData.Name;
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

        var preview = EquipmentInventoryIconScene.Instantiate<EquipmentInventoryIcon>();
        preview.SetEquipment(Equipment);
        preview.SetSlotted(Tick.Visible);
        preview.MouseFilter = MouseFilterEnum.Ignore;
        preview.Size = Size;
        preview.ZIndex = 100;
        SetDragPreview(preview);

        return Equipment.Id.ToString();
    }
}

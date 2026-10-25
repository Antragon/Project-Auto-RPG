namespace Game.scripts.UI;

using System;
using Godot;
using Skills;

public partial class SkillInventoryIcon : Panel
{
    private bool _mouseButtonPressed;

    private bool _wasDragged;

    private TextureRect Icon => field ??= GetNode<TextureRect>("Icon");

    private Panel Tick => field ??= GetNode<Panel>("Tick");

    private Panel SelectionBorder => field ??= GetNode<Panel>("SelectionBorder");

    private PackedScene SkillInventoryIconScene => field ??= SceneRepository.Load("ui/SkillInventoryIcon")!;

    public Skill Skill { get; private set; } = null!;

    public event Action<SkillInventoryIcon>? Clicked;

    public void SetSkill(Skill skill)
    {
        Skill = skill;
        Icon.Texture = skill.SkillData.Icon;
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

        var preview = SkillInventoryIconScene.Instantiate<SkillInventoryIcon>();
        preview.SetSkill(Skill);
        preview.SetSlotted(Tick.Visible);
        preview.MouseFilter = MouseFilterEnum.Ignore;
        preview.Size = Size;
        preview.ZIndex = 100;
        SetDragPreview(preview);

        return Skill.Id.ToString();
    }
}

namespace Game.scripts.UI;

using System;
using Godot;
using Skills;

public partial class CharacterDetailsSkillSlot : Panel
{
    private TextureRect Icon => field ??= GetNode<TextureRect>("Icon");

    private Button RemoveButton => field ??= GetNode<Button>("RemoveButton");

    private StyleBox NormalStyle => field ??= GetThemeStylebox("panel");

    private StyleBoxFlat SelectedStyle => field ??= CreateSelectedStyle();

    private SkillData? _skillData;

    private SkillData? _selectedSkillData;

    [Export]
    public int SlotIndex { get; set; }

    public event Action<int, Guid>? SkillDropped;

    public event Action<int>? UnslotRequested;

    public event Action<SkillData>? Clicked;

    public override void _Ready()
    {
        RemoveButton.Pressed += OnRemoveButtonPressed;
    }

    public override void _ExitTree()
    {
        RemoveButton.Pressed -= OnRemoveButtonPressed;
    }

    public void SetSkill(SkillData? skillData, bool slotted)
    {
        _skillData = skillData;
        Icon.Texture = skillData?.Icon;
        RemoveButton.Visible = slotted;
        MouseDefaultCursorShape = skillData is null ? CursorShape.Arrow : CursorShape.PointingHand;
        RefreshSelection();
    }

    public void SetSelectedSkill(SkillData? skillData)
    {
        _selectedSkillData = skillData;
        RefreshSelection();
    }

    private void RefreshSelection()
    {
        var selected = _skillData is not null && _skillData == _selectedSkillData;
        AddThemeStyleboxOverride("panel", selected ? SelectedStyle : NormalStyle);
    }

    private StyleBoxFlat CreateSelectedStyle()
    {
        var style = (StyleBoxFlat)NormalStyle.Duplicate();
        style.BorderColor = new Color(0.75f, 0.6f, 1);
        style.SetBorderWidthAll(3);
        return style;
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true }
            && _skillData is { } skillData)
        {
            Clicked?.Invoke(skillData);
            AcceptEvent();
        }
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        return TryGetSkillId(data, out _);
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        if (TryGetSkillId(data, out var skillId))
        {
            SkillDropped?.Invoke(SlotIndex, skillId);
        }
    }

    private void OnRemoveButtonPressed()
    {
        UnslotRequested?.Invoke(SlotIndex);
    }

    private static bool TryGetSkillId(Variant data, out Guid skillId)
    {
        if (data.VariantType == Variant.Type.String && Guid.TryParse(data.AsString(), out skillId))
        {
            return true;
        }

        skillId = Guid.Empty;
        return false;
    }
}

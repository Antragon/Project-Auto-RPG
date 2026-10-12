namespace Game.scripts.UI;

using System;
using Godot;
using Skills;

public partial class CharacterDetailsSkillSlot : Panel
{
    private TextureRect Icon => field ??= GetNode<TextureRect>("Icon");

    private Button RemoveButton => field ??= GetNode<Button>("RemoveButton");

    [Export]
    public int SlotIndex { get; set; }

    public event Action<int, SkillData>? SkillDropped;

    public event Action<int>? UnslotRequested;

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
        Icon.Texture = skillData?.Icon;
        RemoveButton.Visible = slotted;
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        return TryGetSkillData(data, out _);
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        if (TryGetSkillData(data, out var skillData))
        {
            SkillDropped?.Invoke(SlotIndex, skillData);
        }
    }

    private void OnRemoveButtonPressed()
    {
        UnslotRequested?.Invoke(SlotIndex);
    }

    private static bool TryGetSkillData(Variant data, out SkillData skillData)
    {
        if (data.VariantType == Variant.Type.Object && data.AsGodotObject() is SkillData skill)
        {
            skillData = skill;
            return true;
        }

        skillData = null!;
        return false;
    }
}

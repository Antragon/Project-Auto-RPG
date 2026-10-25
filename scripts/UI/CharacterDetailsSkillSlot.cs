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

    public event Action<int, Guid>? SkillDropped;

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

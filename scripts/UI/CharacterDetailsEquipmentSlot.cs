namespace Game.scripts.UI;

using System;
using Extensions;
using Godot;
using Equipment;
using EquipmentInstance = Game.scripts.Equipment.Equipment;

public partial class CharacterDetailsEquipmentSlot : Panel
{
    private TextureRect Icon => field ??= GetNode<TextureRect>("Icon");

    private Label Label => field ??= GetNode<Label>("Label");

    private Button RemoveButton => field ??= GetNode<Button>("RemoveButton");

    private EquipmentRepository EquipmentRepository => field ??= this.Root.Get<EquipmentRepository>();

    [Export]
    public EquipmentType EquipmentType { get; set; }

    public event Action<EquipmentType, Guid>? EquipmentDropped;

    public event Action<EquipmentType>? UnslotRequested;

    public override void _Ready()
    {
        RemoveButton.Pressed += OnRemoveButtonPressed;
    }

    public override void _ExitTree()
    {
        RemoveButton.Pressed -= OnRemoveButtonPressed;
    }

    public void SetEquipment(EquipmentInstance? equipment)
    {
        Icon.Texture = equipment?.EquipmentData.Icon;
        Label.Visible = equipment is null;
        RemoveButton.Visible = equipment is not null;
        TooltipText = equipment?.EquipmentData.Name ?? string.Empty;
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        return TryGetEquipment(data, out var equipment) && equipment.EquipmentData.Type == EquipmentType;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        if (TryGetEquipment(data, out var equipment) && equipment.EquipmentData.Type == EquipmentType)
        {
            EquipmentDropped?.Invoke(EquipmentType, equipment.Id);
        }
    }

    private void OnRemoveButtonPressed()
    {
        UnslotRequested?.Invoke(EquipmentType);
    }

    private bool TryGetEquipment(Variant data, out EquipmentInstance equipment)
    {
        equipment = null!;
        if (data.VariantType != Variant.Type.String
            || !Guid.TryParse(data.AsString(), out var equipmentId)
            || !EquipmentRepository.TryGetEquipment(equipmentId, out var foundEquipment))
        {
            return false;
        }

        equipment = foundEquipment;
        return true;
    }
}

namespace Game.scripts.UI;

using System;
using Extensions;
using Godot;
using Equipment;
using Units;

public partial class EquipmentPanel : Panel
{
    private GridContainer EquipmentGrid => field ??= GetNode<GridContainer>("Scroll/Grid");

    private CharacterRepository CharacterRepository => field ??= this.Root.Get<CharacterRepository>();

    private EquipmentRepository EquipmentRepository => field ??= this.Root.Get<EquipmentRepository>();

    private EquipmentAssignments EquipmentAssignments => field ??= this.Root.Get<EquipmentAssignments>();

    private PackedScene EquipmentInventoryIconScene => field ??= SceneRepository.Load("ui/EquipmentInventoryIcon")!;

    public EquipmentInventoryIcon[] Icons => EquipmentGrid.GetChildrenOfType<EquipmentInventoryIcon>();

    public override void _Ready()
    {
        PopulateEquipments();
        foreach (var character in CharacterRepository.Characters.Values)
        {
            character.PropertyChanged += OnCharacterPropertyChanged;
        }

        RefreshSlottedStates();
    }

    public override void _ExitTree()
    {
        foreach (var character in CharacterRepository.Characters.Values)
        {
            character.PropertyChanged -= OnCharacterPropertyChanged;
        }
    }

    private void PopulateEquipments()
    {
        foreach (var child in EquipmentGrid.GetChildren())
        {
            child.QueueFree();
        }

        foreach (var equipment in EquipmentRepository.Equipments.Values)
        {
            var icon = EquipmentInventoryIconScene.Instantiate<EquipmentInventoryIcon>();
            icon.SetEquipment(equipment);
            EquipmentGrid.AddChild(icon);
        }
    }

    private void OnCharacterPropertyChanged(Character character, string propertyName)
    {
        if (Enum.TryParse<EquipmentType>(propertyName, out _))
        {
            RefreshSlottedStates();
        }
    }

    private void RefreshSlottedStates()
    {
        foreach (var icon in Icons)
        {
            icon.SetSlotted(EquipmentAssignments.TryGet(icon.Equipment.Id, out _));
        }
    }
}

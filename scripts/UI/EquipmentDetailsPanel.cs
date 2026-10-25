namespace Game.scripts.UI;

using System;
using Extensions;
using Equipment;
using Godot;

public partial class EquipmentDetailsPanel : Panel
{
    private Label EquipmentName => field ??= GetNode<Label>("EquipmentName");

    private Label EquipmentType => field ??= GetNode<Label>("Details/TypeRow/TypeValue");

    private Label PowerValue => field ??= GetNode<Label>("Details/PowerRow/PowerValue");

    private Label DefenseValue => field ??= GetNode<Label>("Details/DefenseRow/DefenseValue");

    private Label MagicDefenseValue => field ??= GetNode<Label>("Details/MagicDefenseRow/MagicDefenseValue");

    private Button CloseButton => field ??= GetNode<Button>("CloseButton");

    public event Action? Closed;

    public override void _Ready()
    {
        CloseButton.Pressed += OnCloseButtonPressed;
    }

    public override void _ExitTree()
    {
        CloseButton.Pressed -= OnCloseButtonPressed;
    }

    public void ShowEquipment(Equipment equipment)
    {
        var equipmentData = equipment.EquipmentData;
        EquipmentName.Text = equipmentData.Name;
        EquipmentType.Text = equipmentData.Type.ToDisplayName();
        PowerValue.Text = equipmentData.Power.ToString();
        DefenseValue.Text = equipmentData.Defense.ToString();
        MagicDefenseValue.Text = equipmentData.MagicDefense.ToString();
        Visible = true;
    }

    public void HideEquipment()
    {
        Visible = false;
    }

    private void OnCloseButtonPressed()
    {
        HideEquipment();
        Closed?.Invoke();
    }
}

internal static class EquipmentTypeExtensions
{
    public static string ToDisplayName(this EquipmentType equipmentType)
    {
        return equipmentType switch
        {
            EquipmentType.BodyArmor => "Body armor",
            EquipmentType.LegArmor => "Leg armor",
            EquipmentType.Accessory1 => "Accessory 1",
            EquipmentType.Accessory2 => "Accessory 2",
            _ => equipmentType.ToString(),
        };
    }
}

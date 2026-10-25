namespace Game.scripts.Equipment;

using Godot;

public static class EquipmentDataRepository
{
    public static EquipmentData? Load(string name)
    {
        var equipmentData = GD.Load<EquipmentData>($"res://resources/equipments/{name}.tres");
        if (equipmentData is null)
        {
            GD.PushWarning($"Could not load equipment '{name}'.");
        }

        return equipmentData;
    }
}

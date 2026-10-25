namespace Game.scripts.Equipment;

using System;

public sealed class Equipment(Guid id, EquipmentData equipmentData)
{
    public Guid Id { get; } = id;

    public EquipmentData EquipmentData { get; } = equipmentData;
}

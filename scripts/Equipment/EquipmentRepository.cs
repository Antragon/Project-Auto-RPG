namespace Game.scripts.Equipment;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

public sealed class EquipmentRepository
{
    private readonly Dictionary<Guid, Equipment> _equipments;

    private readonly ReadOnlyDictionary<Guid, Equipment> _readOnlyEquipments;

    public EquipmentRepository(Dictionary<Guid, Equipment> equipments)
    {
        _equipments = equipments;
        _readOnlyEquipments = new ReadOnlyDictionary<Guid, Equipment>(_equipments);
    }

    public IReadOnlyDictionary<Guid, Equipment> Equipments => _readOnlyEquipments;

    public Equipment Add(EquipmentData equipmentData)
    {
        var equipment = new Equipment(Guid.NewGuid(), equipmentData);
        _equipments.Add(equipment.Id, equipment);
        return equipment;
    }

    public bool TryGetEquipment(Guid equipmentId, [NotNullWhen(true)] out Equipment? equipment)
    {
        return _equipments.TryGetValue(equipmentId, out equipment);
    }
}

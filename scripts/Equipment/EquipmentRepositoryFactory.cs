namespace Game.scripts.Equipment;

using System;
using System.Collections.Generic;
using Persistence;

public sealed class EquipmentRepositoryFactory(SaveRepository saveRepository)
{
    public EquipmentRepository Create()
    {
        var equipments = new Dictionary<Guid, Equipment>();
        foreach (var savedEquipment in saveRepository.SaveData.Equipments)
        {
            var equipmentData = EquipmentDataRepository.Load(savedEquipment.Name);
            if (equipmentData is not null)
            {
                equipments.Add(savedEquipment.Id, new Equipment(savedEquipment.Id, equipmentData));
            }
        }

        return new EquipmentRepository(equipments);
    }
}

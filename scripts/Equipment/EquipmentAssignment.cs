namespace Game.scripts.Equipment;

using System;

public sealed record EquipmentAssignment(Guid CharacterId, EquipmentType EquipmentType);

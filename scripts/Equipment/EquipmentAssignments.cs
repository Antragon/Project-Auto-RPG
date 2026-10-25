namespace Game.scripts.Equipment;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

public sealed class EquipmentAssignments
{
    private readonly Dictionary<Guid, EquipmentAssignment> _assignmentsByEquipmentId = [];

    private readonly Dictionary<EquipmentAssignment, Guid> _equipmentIdsByAssignment = [];

    public void Set(Guid equipmentId, EquipmentAssignment assignment)
    {
        if (_equipmentIdsByAssignment.TryGetValue(assignment, out var previousEquipmentId))
        {
            _assignmentsByEquipmentId.Remove(previousEquipmentId);
        }

        if (_assignmentsByEquipmentId.TryGetValue(equipmentId, out var previousAssignment))
        {
            _equipmentIdsByAssignment.Remove(previousAssignment);
        }

        _assignmentsByEquipmentId[equipmentId] = assignment;
        _equipmentIdsByAssignment[assignment] = equipmentId;
    }

    public void Remove(EquipmentAssignment assignment)
    {
        if (_equipmentIdsByAssignment.Remove(assignment, out var equipmentId))
        {
            _assignmentsByEquipmentId.Remove(equipmentId);
        }
    }

    public bool TryGet(Guid equipmentId, [NotNullWhen(true)] out EquipmentAssignment? assignment)
    {
        return _assignmentsByEquipmentId.TryGetValue(equipmentId, out assignment);
    }
}

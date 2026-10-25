namespace Game.scripts.UI;

using System;
using Extensions;
using Godot;
using Equipment;
using Units;
using EquipmentInstance = Game.scripts.Equipment.Equipment;

public partial class CharacterDetailsEquipmentPanel : Panel
{
    private CharacterDetailsEquipmentSlot[] Slots => field ??= GetNode<GridContainer>("Content/EquipmentGrid")
        .GetChildrenOfType<CharacterDetailsEquipmentSlot>();

    private CharacterRepository CharacterRepository => field ??= this.Root.Get<CharacterRepository>();

    private EquipmentRepository EquipmentRepository => field ??= this.Root.Get<EquipmentRepository>();

    private EquipmentAssignments EquipmentAssignments => field ??= this.Root.Get<EquipmentAssignments>();

    private Character? _character;

    public override void _Ready()
    {
        foreach (var slot in Slots)
        {
            slot.EquipmentDropped += OnEquipmentDropped;
            slot.UnslotRequested += OnUnslotRequested;
        }
    }

    public override void _ExitTree()
    {
        foreach (var slot in Slots)
        {
            slot.EquipmentDropped -= OnEquipmentDropped;
            slot.UnslotRequested -= OnUnslotRequested;
        }

        UnbindCharacter();
    }

    public void SetCharacter(Guid? characterId)
    {
        UnbindCharacter();
        if (characterId is not { } id || !CharacterRepository.TryGetCharacter(id, out _character))
        {
            _character = null;
            RefreshEquipment();
            return;
        }

        _character.PropertyChanged += OnCharacterPropertyChanged;
        RefreshEquipment();
    }

    private void OnCharacterPropertyChanged(Character character, string propertyName)
    {
        if (Enum.TryParse<EquipmentType>(propertyName, out _))
        {
            RefreshEquipment();
        }
    }

    private void RefreshEquipment()
    {
        foreach (var slot in Slots)
        {
            slot.SetEquipment(_character?.GetEquipment(slot.EquipmentType));
        }
    }

    private void OnEquipmentDropped(EquipmentType equipmentType, Guid equipmentId)
    {
        if (_character is null || !EquipmentRepository.TryGetEquipment(equipmentId, out var equipment))
        {
            return;
        }

        SetEquipment(_character, equipmentType, equipment);
    }

    private void OnUnslotRequested(EquipmentType equipmentType)
    {
        if (_character is not null)
        {
            SetEquipment(_character, equipmentType, null);
        }
    }

    private void SetEquipment(Character character, EquipmentType equipmentType, EquipmentInstance? equipment)
    {
        if (equipment?.EquipmentData.Type != equipmentType && equipment is not null)
        {
            return;
        }

        if (character.GetEquipment(equipmentType)?.Id == equipment?.Id)
        {
            return;
        }

        if (equipment is not null
            && EquipmentAssignments.TryGet(equipment.Id, out var previousAssignment)
            && CharacterRepository.TryGetCharacter(previousAssignment.CharacterId, out var previousCharacter))
        {
            previousCharacter.SetEquipment(previousAssignment.EquipmentType, null);
        }

        character.SetEquipment(equipmentType, equipment);
    }

    private void UnbindCharacter()
    {
        if (_character is null)
        {
            return;
        }

        _character.PropertyChanged -= OnCharacterPropertyChanged;
        _character = null;
    }
}

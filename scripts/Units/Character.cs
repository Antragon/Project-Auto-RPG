namespace Game.scripts.Units;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Equipment;

public sealed class Character
{
    private const int BaseXpPerLevel = 10;

    public Character(Guid id, Unit unit, int xp = 0)
    {
        Id = id;
        Unit = unit;
        Xp = xp;
    }

    public Guid Id { get; }

    public Unit Unit { get; }

    public Equipment? Helmet
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public Equipment? BodyArmor
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public Equipment? LegArmor
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public Equipment? Weapon
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public Equipment? Accessory1
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public Equipment? Accessory2
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public int Xp
    {
        get;
        private set
        {
            var newXp = Math.Max(0, value);
            if (field == newXp)
            {
                return;
            }

            field = newXp;
            Unit.Level = CalculateLevel();
            PropertyChanged?.Invoke(this, nameof(Xp));
        }
    }

    public event PropertyChangedEventHandler<Character>? PropertyChanged;

    public Equipment? GetEquipment(EquipmentType equipmentType)
    {
        return equipmentType switch
        {
            EquipmentType.Helmet => Helmet,
            EquipmentType.BodyArmor => BodyArmor,
            EquipmentType.LegArmor => LegArmor,
            EquipmentType.Weapon => Weapon,
            EquipmentType.Accessory1 => Accessory1,
            EquipmentType.Accessory2 => Accessory2,
            _ => throw new ArgumentOutOfRangeException(nameof(equipmentType), equipmentType, null),
        };
    }

    public void SetEquipment(EquipmentType equipmentType, Equipment? equipment)
    {
        if (equipment is not null && equipment.EquipmentData.Type != equipmentType)
        {
            throw new ArgumentException("Equipment type does not match the character slot.", nameof(equipment));
        }

        switch (equipmentType)
        {
            case EquipmentType.Helmet:
                Helmet = equipment;
                break;
            case EquipmentType.BodyArmor:
                BodyArmor = equipment;
                break;
            case EquipmentType.LegArmor:
                LegArmor = equipment;
                break;
            case EquipmentType.Weapon:
                Weapon = equipment;
                break;
            case EquipmentType.Accessory1:
                Accessory1 = equipment;
                break;
            case EquipmentType.Accessory2:
                Accessory2 = equipment;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(equipmentType), equipmentType, null);
        }
    }

    public int XpInCurrentLevel => Xp - CalculateXpThresholdForLevel(Unit.Level - 1);

    public int XpToNextLevel => BaseXpPerLevel * Fibonacci.Get(Unit.Level - 1);

    public void AddXp(int xp)
    {
        if (xp < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(xp), xp, "XP cannot be negative.");
        }

        Xp += xp;
    }

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, name);
    }

    private int CalculateLevel()
    {
        var level = 1;
        while (Xp >= CalculateXpThresholdForLevel(level))
        {
            level++;
        }

        return level;
    }

    private static int CalculateXpThresholdForLevel(int level)
    {
        return level == 0
            ? 0
            : BaseXpPerLevel * Fibonacci.GetCumulative(level - 1);
    }
}

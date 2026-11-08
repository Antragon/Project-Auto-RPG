namespace Game.scripts.Units;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Skills;

public sealed class Unit
{
    public Unit(UnitData unitData)
    {
        UnitData = unitData;
        VitalityModifier.Changed += () => OnBaseStatChanged(nameof(Vitality));
        StrengthModifier.Changed += () => OnBaseStatChanged(nameof(Strength));
        DexterityModifier.Changed += () => OnBaseStatChanged(nameof(Dexterity));
        MagicModifier.Changed += () => OnBaseStatChanged(nameof(Magic));
        SpeedModifier.Changed += () => OnBaseStatChanged(nameof(Speed));
        MaxHp = CalculateMaxHp();
        Hp = MaxHp;
    }

    public UnitData UnitData { get; }

    public StatModifier Power { get; } = new();

    public StatModifier Defense { get; } = new();

    public StatModifier MagicDefense { get; } = new();

    public StatModifier VitalityModifier { get; } = new();

    public StatModifier StrengthModifier { get; } = new();

    public StatModifier DexterityModifier { get; } = new();

    public StatModifier MagicModifier { get; } = new();

    public StatModifier SpeedModifier { get; } = new();

    public Skill? Skill1
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Skill? Skill2
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Skill? Skill3
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Skill? Skill4
    {
        get;
        set => SetProperty(ref field, value);
    }

    public int Vitality => UnitData.Vitality * Level + VitalityModifier.Value;

    public int Strength => UnitData.Strength * Level + StrengthModifier.Value;

    public int Dexterity => UnitData.Dexterity * Level + DexterityModifier.Value;

    public int Magic => UnitData.Magic * Level + MagicModifier.Value;

    public int Speed => UnitData.Speed * Level + SpeedModifier.Value;

    public int Level
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            MaxHp = CalculateMaxHp();
            if (Hp > MaxHp) SetHp(MaxHp);
            PropertyChanged?.Invoke(this, nameof(Level));
        }
    } = 1;

    public int Hp
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public int MaxHp
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public bool IsDead
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public event PropertyChangedEventHandler<Unit>? PropertyChanged;

    private void OnBaseStatChanged(string propertyName)
    {
        if (propertyName == nameof(Vitality))
        {
            MaxHp = CalculateMaxHp();
            if (Hp > MaxHp) SetHp(MaxHp);
        }

        PropertyChanged?.Invoke(this, propertyName);
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

    private int CalculateMaxHp()
    {
        return (int)Math.Floor(100 * Math.Pow(1.1, Vitality));
    }

    public void SetHp(int hp)
    {
        var newHp = Math.Clamp(hp, 0, MaxHp);
        if (Hp == newHp)
        {
            return;
        }

        Hp = newHp;
        if (Hp == 0)
        {
            IsDead = true;
        }
        else if (Hp == MaxHp)
        {
            IsDead = false;
        }
    }
}

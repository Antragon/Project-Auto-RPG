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
        MaxHp = CalculateMaxHp();
        Hp = MaxHp;
    }

    public UnitData UnitData { get; }

    public SkillData? Skill1
    {
        get;
        set => SetProperty(ref field, value);
    }

    public SkillData? Skill2
    {
        get;
        set => SetProperty(ref field, value);
    }

    public SkillData? Skill3
    {
        get;
        set => SetProperty(ref field, value);
    }

    public SkillData? Skill4
    {
        get;
        set => SetProperty(ref field, value);
    }

    public int Vitality => UnitData.Vitality * Level;

    public int Strength => UnitData.Strength * Level;

    public int Dexterity => UnitData.Dexterity * Level;

    public int Magic => UnitData.Magic * Level;

    public int Speed => UnitData.Speed * Level;

    public int Level
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            MaxHp = CalculateMaxHp();
            Hp = MaxHp;
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

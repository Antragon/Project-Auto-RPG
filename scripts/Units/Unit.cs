namespace Game.scripts.Units;

using System;

public sealed class Unit
{
    public Unit(UnitData unitData)
    {
        UnitData = unitData;
        MaxHp = CalculateMaxHp();
        Hp = MaxHp;
    }

    public UnitData UnitData { get; }

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
        private set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, nameof(Hp));
        }
    }

    public int MaxHp
    {
        get;
        private set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, nameof(MaxHp));
        }
    }

    public bool IsDead
    {
        get;
        private set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, nameof(IsDead));
        }
    }

    public event PropertyChangedEventHandler<Unit>? PropertyChanged;

    private int CalculateMaxHp()
    {
        return (int)Math.Floor(100 * Math.Pow(1.1, UnitData.Vitality * Level));
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

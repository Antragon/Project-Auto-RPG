namespace Game.scripts.Units;

using System;

public sealed class Unit
{
    public Unit(UnitData unitData)
    {
        UnitData = unitData;
        Hp = MaxHp;
    }

    public UnitData UnitData { get; }

    public int Level
    {
        get;
        internal set
        {
            if (field == value)
            {
                return;
            }

            field = Math.Max(1, value);
            Hp = MaxHp;
            IsDead = false;
            HpChanged?.Invoke();
        }
    } = 1;

    public int Hp { get; private set; }

    public int MaxHp => CalculateMaxHp();

    public bool IsDead { get; private set; }

    public event Action? HpChanged;

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

        HpChanged?.Invoke();
    }
}

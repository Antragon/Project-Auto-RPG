namespace Game.scripts.Units;

using System;

public sealed class Unit
{
    public Unit(UnitData unitData)
    {
        UnitData = unitData;
        MaxHp = (int)Math.Floor(100 * Math.Pow(1.1, unitData.Vitality));
        Hp = MaxHp;
    }

    public UnitData UnitData { get; }

    public int Hp { get; private set; }

    public int MaxHp { get; }

    public bool IsDead { get; private set; }

    public event Action? HpChanged;

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

namespace Game.scripts.Units;

using System;

public sealed class Unit
{
    public Unit(UnitData unitData)
    {
        UnitData = unitData;
        MaxHp = 100 + unitData.Vitality * 10;
        Hp = MaxHp;
    }

    public UnitData UnitData { get; }

    public int Hp { get; private set; }

    public int MaxHp { get; }

    public bool IsDead => Hp <= 0;

    public event Action? Changed;

    public void SetHp(int hp)
    {
        var newHp = Math.Clamp(hp, 0, MaxHp);
        if (Hp == newHp)
        {
            return;
        }

        Hp = newHp;
        Changed?.Invoke();
    }
}
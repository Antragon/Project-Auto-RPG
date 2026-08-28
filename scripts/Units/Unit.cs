namespace Game.scripts.Units;

public sealed class Unit
{
    public Unit(UnitData unitData)
    {
        UnitData = unitData;
        Hp = MaxHp = 100 + unitData.Vitality * 10;
    }

    public UnitData UnitData { get; }

    public int Hp { get; }

    public int MaxHp { get; }
}

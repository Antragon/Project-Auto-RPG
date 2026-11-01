namespace Game.scripts.Units;

public sealed class Enemy
{
    public Enemy(EnemyData enemyData, Unit unit)
    {
        EnemyData = enemyData;
        Unit = unit;
    }

    public EnemyData EnemyData { get; }

    public Unit Unit { get; }
}

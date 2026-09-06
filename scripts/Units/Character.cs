namespace Game.scripts.Units;

using System;

public sealed class Character
{
    private const int BaseXpPerLevel = 10;

    public Character(Unit unit, int xp = 0)
    {
        Unit = unit;
        Xp = xp;
    }

    public Unit Unit { get; }

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

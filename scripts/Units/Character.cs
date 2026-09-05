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
            field = Math.Max(0, value);
            Unit.Level = CalculateLevel(field);
        }
    }

    public int XpToNextLevel => CalculateTotalXpForLevel(Unit.Level + 1);

    public int XpUntilLevelUp => XpToNextLevel - Xp;

    public void AddXp(int xp)
    {
        if (xp < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(xp), xp, "XP cannot be negative.");
        }

        Xp += xp;
    }

    private static int CalculateLevel(int xp)
    {
        var level = 1;
        while (xp >= CalculateTotalXpForLevel(level + 1))
        {
            level++;
        }

        return level;
    }

    private static int CalculateTotalXpForLevel(int level)
    {
        return BaseXpPerLevel * (level - 1) * level / 2;
    }
}

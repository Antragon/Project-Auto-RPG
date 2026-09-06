namespace Game.scripts;

using System;

public static class Fibonacci
{
    public static int Get(int index)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, "The Fibonacci index cannot be negative.");
        }

        var previous = 1;
        var current = 2;
        for (var currentIndex = 0; currentIndex < index; currentIndex++)
        {
            (previous, current) = (current, previous + current);
        }

        return previous;
    }

    public static int GetCumulative(int index)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, "The Fibonacci index cannot be negative.");
        }

        var cumulativeValue = 0;
        var previous = 1;
        var current = 2;
        for (var currentIndex = 0; currentIndex <= index; currentIndex++)
        {
            cumulativeValue += previous;
            (previous, current) = (current, previous + current);
        }

        return cumulativeValue;
    }
}

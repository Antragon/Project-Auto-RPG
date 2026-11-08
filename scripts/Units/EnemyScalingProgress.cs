namespace Game.scripts.Units;

public readonly record struct EnemyScalingProgress(int Bonus, int? NextThreshold)
{
    private const int DefeatsPerMilestone = 10;

    // Fibonacci.Get(44) is the last Fibonacci value representable as an int.
    private const int LastMilestoneIndex = 44;

    public int LastThreshold => Bonus == 0 ? 0 : Fibonacci.Get(Bonus - 1) * DefeatsPerMilestone;

    public static EnemyScalingProgress FromDefeatCount(int defeatCount)
    {
        var bonus = 0;
        while (bonus <= LastMilestoneIndex)
        {
            var threshold = (long)Fibonacci.Get(bonus) * DefeatsPerMilestone;
            if (threshold > int.MaxValue)
            {
                break;
            }

            if (defeatCount < threshold)
            {
                return new EnemyScalingProgress(bonus, (int)threshold);
            }

            bonus++;
        }

        return new EnemyScalingProgress(bonus, null);
    }
}

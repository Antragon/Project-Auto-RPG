namespace Game.scripts.Units;

public readonly record struct EnemyScalingProgress(int Bonus, int? NextThreshold)
{
    // Fibonacci.Get(44) is the last milestone representable by the saved int defeat count.
    private const int LastMilestoneIndex = 44;

    public static EnemyScalingProgress FromDefeatCount(int defeatCount)
    {
        var bonus = 0;
        while (bonus <= LastMilestoneIndex)
        {
            var threshold = Fibonacci.Get(bonus);
            if (defeatCount < threshold)
            {
                return new EnemyScalingProgress(bonus, threshold);
            }

            bonus++;
        }

        return new EnemyScalingProgress(bonus, null);
    }
}

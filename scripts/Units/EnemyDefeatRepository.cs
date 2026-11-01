namespace Game.scripts.Units;

using System;
using System.Collections.Generic;
using Persistence;

public sealed class EnemyDefeatRepository(SaveRepository saveRepository)
{
    public event Action? Changed;

    public int GetCount(string enemyName)
    {
        return saveRepository.SaveData.EnemyDefeatCounts.GetValueOrDefault(enemyName);
    }

    public void RecordDefeat(string enemyName)
    {
        var count = GetCount(enemyName);
        if (count == int.MaxValue) return;

        saveRepository.SaveData.EnemyDefeatCounts[enemyName] = count + 1;
        Changed?.Invoke();
    }
}

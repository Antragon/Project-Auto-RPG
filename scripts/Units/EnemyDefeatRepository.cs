namespace Game.scripts.Units;

using System;
using System.Collections.Generic;
using Persistence;

public sealed class EnemyDefeatRepository(SaveRepository saveRepository, SaveDataWriter saveDataWriter)
{
    public event Action? Changed;

    public int GetCount(string enemyName)
    {
        return saveRepository.SaveData.EnemyDefeatCounts.GetValueOrDefault(enemyName);
    }

    public void RecordDefeat(string enemyName)
    {
        saveRepository.SaveData.EnemyDefeatCounts[enemyName] = GetCount(enemyName) + 1;
        saveDataWriter.Save();
        Changed?.Invoke();
    }
}

namespace Game.scripts.Loot;

using System;
using System.Collections.Generic;
using System.Linq;
using Persistence;

public sealed class LootRepository(SaveRepository saveRepository)
{
    public event Action? Changed;

    public IReadOnlyList<SavedLoot> Loot => saveRepository.SaveData.Progress.Loot;

    public void Add(string name, int amount)
    {
        if (string.IsNullOrWhiteSpace(name) || amount <= 0)
        {
            return;
        }

        var savedLoot = saveRepository.SaveData.Progress.Loot.FirstOrDefault(loot => loot.Name == name);
        if (savedLoot is null)
        {
            savedLoot = new SavedLoot { Name = name };
            saveRepository.SaveData.Progress.Loot.Add(savedLoot);
        }

        savedLoot.Amount += amount;
        Changed?.Invoke();
    }
}

namespace Game.scripts.Loot;

using Godot;

public static class LootDataRepository
{
    public static LootData? Load(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        var lootData = GD.Load<LootData>($"res://resources/loot/{name}.tres");
        if (lootData is null)
        {
            GD.PushWarning($"Could not load loot '{name}'.");
        }

        return lootData;
    }
}

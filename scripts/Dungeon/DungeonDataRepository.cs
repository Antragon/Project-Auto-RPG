namespace Game.scripts.Dungeon;

using Godot;

public static class DungeonDataRepository
{
    public static DungeonData? Load(string name)
    {
        var dungeonData = GD.Load<DungeonData>($"res://resources/dungeons/{name}.tres");
        if (dungeonData is null)
        {
            GD.PushWarning($"Could not load dungeon '{name}'.");
        }

        return dungeonData;
    }
}

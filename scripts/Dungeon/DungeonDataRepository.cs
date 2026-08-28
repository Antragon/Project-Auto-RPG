namespace Game.scripts.Dungeon;

using Godot;

public static class DungeonDataRepository
{
    public static DungeonData? Load(string name)
    {
        return GD.Load<DungeonData>($"res://resources/dungeons/{name}.tres");
    }
}

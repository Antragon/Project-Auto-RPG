namespace Game.scripts;

using Godot;

public static class SceneRepository
{
    public static PackedScene? Load(string name)
    {
        return GD.Load<PackedScene>($"res://scenes/{name}.tscn");
    }
}

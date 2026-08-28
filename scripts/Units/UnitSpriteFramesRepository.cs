namespace Game.scripts.Units;

using Godot;

public static class UnitSpriteFramesRepository
{
    public static SpriteFrames? Load(string name)
    {
        return GD.Load<SpriteFrames>($"res://textures/units/{name}.tres");
    }
}

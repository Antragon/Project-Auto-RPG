namespace Game.scripts.Units;

using Godot;

public static class UnitSpriteFramesRepository
{
    public static SpriteFrames? Load(string name)
    {
        var spriteFrames = GD.Load<SpriteFrames>($"res://textures/units/{name}.tres");
        if (spriteFrames is null)
        {
            GD.PushWarning($"Could not load SpriteFrames for character '{name}'.");
        }

        return spriteFrames;
    }
}

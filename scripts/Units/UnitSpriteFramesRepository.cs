namespace Game.scripts.Units;

using System.Diagnostics.CodeAnalysis;
using Godot;

public static class UnitSpriteFramesRepository
{
    public static bool TryLoad(string name, [NotNullWhen(true)] out SpriteFrames? spriteFrames)
    {
        spriteFrames = Load(name);
        return spriteFrames != null;
    }

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

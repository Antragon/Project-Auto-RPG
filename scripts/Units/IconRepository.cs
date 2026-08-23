namespace Game.scripts.Units;

using Godot;

public static class IconRepository
{
    public static Texture2D? GetUnit(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        var icons = Load(name) ?? Load("placeholder");
        return icons;
    }

    private static Texture2D? Load(string name)
    {
        return GD.Load<Texture2D>($"res://textures/units/icons/{name}.tres");
    }
}
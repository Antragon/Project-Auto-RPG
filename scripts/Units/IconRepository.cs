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

        const string basePath = "units/icons/";
        var icons = Load(basePath + name) ?? Load(basePath + "placeholder");
        return icons;
    }

    public static Texture2D? GetUI(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return Load("ui/" + name);
    }

    private static Texture2D? Load(string path)
    {
        return GD.Load<Texture2D>($"res://textures/{path}.tres");
    }
}

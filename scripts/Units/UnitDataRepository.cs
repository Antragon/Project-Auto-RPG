namespace Game.scripts.Units;

using Godot;

public static class UnitDataRepository
{
    public static UnitData? Load(string name)
    {
        return GD.Load<UnitData>($"res://resources/units/{name}.tres");
    }
}

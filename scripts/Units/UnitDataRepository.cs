namespace Game.scripts.Units;

using Godot;

public static class UnitDataRepository
{
    public static UnitData? Load(string name)
    {
        var unitData = GD.Load<UnitData>($"res://resources/units/{name}.tres");

        if (unitData is null)
        {
            GD.PushWarning($"Could not load unit '{name}'.");
        }

        return unitData;
    }
}

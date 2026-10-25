namespace Game.scripts.Equipment;

using Godot;

public enum EquipmentType
{
    Helmet,
    BodyArmor,
    LegArmor,
    Weapon,
    Accessory1,
    Accessory2,
}

[GlobalClass]
public partial class EquipmentData : Resource
{
    [Export] public Texture2D? Icon { get; set; }

    [Export] public int Power { get; set; }

    [Export] public int Defense { get; set; }

    [Export] public int MagicDefense { get; set; }

    [Export] public EquipmentType Type { get; set; }
}

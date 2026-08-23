namespace Game.scripts.UI;

using Godot;
using Units;

public partial class InventoryIcon : TextureRect
{
    public void SetCharacter(string name)
    {
        Texture = IconRepository.GetUnit(name);
    }
}
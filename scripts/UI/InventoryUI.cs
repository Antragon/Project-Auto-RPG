namespace Game.scripts.UI;

using Extensions;
using Godot;
using Persistence;

public partial class InventoryUI : Panel
{
    private SaveRepository SaveRepository => field ??= this.Root.Get<SaveRepository>();

    private GridContainer CharactersGrid => field ??= GetNode<GridContainer>("CharactersPanel/Scroll/Grid");

    private PackedScene InventoryIconScene => field ??= GD.Load<PackedScene>("res://scenes/ui/InventoryIcon.tscn")!;

    public override void _Ready()
    {
        PopulateCharacters();
    }

    private void PopulateCharacters()
    {
        foreach (var child in CharactersGrid.GetChildren())
        {
            child.Free();
        }

        foreach (var character in SaveRepository.Characters)
        {
            var icon = InventoryIconScene.Instantiate<InventoryIcon>();
            CharactersGrid.AddChild(icon);
            icon.SetCharacter(character.Name);
        }
    }
}
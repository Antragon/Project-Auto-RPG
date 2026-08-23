namespace Game.scripts.UI;

using Extensions;
using Godot;
using Persistence;
using Units;

public partial class InventoryUI : Panel
{
    private SaveRepository SaveRepository => field ??= this.Root.Get<SaveRepository>();

    private CharacterFormationService CharacterFormation => field ??= this.Root.Get<CharacterFormationService>();

    private GridContainer CharactersGrid => field ??= GetNode<GridContainer>("CharactersPanel/Scroll/Grid");

    private PackedScene InventoryIconScene => field ??= GD.Load<PackedScene>("res://scenes/ui/InventoryIcon.tscn")!;

    public override void _Ready()
    {
        CharacterFormation.Changed += RefreshSlottedStates;
        PopulateCharacters();
    }

    public override void _ExitTree()
    {
        CharacterFormation.Changed -= RefreshSlottedStates;
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

        RefreshSlottedStates();
    }

    private void RefreshSlottedStates()
    {
        foreach (var child in CharactersGrid.GetChildren())
        {
            if (child is InventoryIcon icon)
            {
                icon.SetSlotted(CharacterFormation.IsSlotted(icon.CharacterName));
            }
        }
    }
}

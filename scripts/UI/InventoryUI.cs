namespace Game.scripts.UI;

using Extensions;
using Godot;
using Persistence;
using Units;

public partial class InventoryUI : Panel
{
    private SaveRepository SaveRepository => field ??= this.Root.Get<SaveRepository>();

    private CharacterRoster CharacterRoster => field ??= this.Root.Get<CharacterRoster>();

    private CharacterFormation CharacterFormation => field ??= this.Root.Get<CharacterFormation>();

    private GridContainer CharactersGrid => field ??= GetNode<GridContainer>("CharactersPanel/Scroll/Grid");

    private PackedScene CharacterInventoryIconScene => field ??= SceneRepository.Load("ui/CharacterInventoryIcon")!;

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
            if (!CharacterRoster.TryGetCharacter(character.Name, out var playableCharacter))
            {
                continue;
            }

            var icon = CharacterInventoryIconScene.Instantiate<CharacterInventoryIcon>();
            CharactersGrid.AddChild(icon);
            icon.SetCharacter(character.Name, playableCharacter);
        }

        RefreshSlottedStates();
    }

    private void RefreshSlottedStates()
    {
        foreach (var child in CharactersGrid.GetChildren())
        {
            if (child is CharacterInventoryIcon icon)
            {
                icon.SetSlotted(CharacterFormation.IsSlotted(icon.CharacterName));
            }
        }
    }
}

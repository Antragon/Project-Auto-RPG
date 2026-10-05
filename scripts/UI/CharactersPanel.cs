namespace Game.scripts.UI;

using Extensions;
using Godot;
using Persistence;
using Units;

public partial class CharactersPanel : Panel
{
    private GridContainer CharactersGrid => field ??= GetNode<GridContainer>("Scroll/Grid");

    private SaveRepository SaveRepository => field ??= this.Root.Get<SaveRepository>();

    private CharacterRoster CharacterRoster => field ??= this.Root.Get<CharacterRoster>();

    private CharacterFormation CharacterFormation => field ??= this.Root.Get<CharacterFormation>();

    private PackedScene CharacterInventoryIconScene => field ??= SceneRepository.Load("ui/CharacterInventoryIcon")!;

    public CharacterInventoryIcon[] Icons => CharactersGrid.GetChildrenOfType<CharacterInventoryIcon>();

    public override void _Ready()
    {
        PopulateCharacters();
        CharacterFormation.Changed += RefreshSlottedStates;
        RefreshSlottedStates();
    }

    public override void _ExitTree()
    {
        CharacterFormation.Changed -= RefreshSlottedStates;
    }

    private void PopulateCharacters()
    {
        foreach (var child in CharactersGrid.GetChildren())
        {
            child.QueueFree();
        }

        foreach (var character in SaveRepository.Characters)
        {
            if (!CharacterRoster.TryGetCharacter(character.Name, out var playableCharacter))
            {
                continue;
            }

            var icon = CharacterInventoryIconScene.Instantiate<CharacterInventoryIcon>();
            icon.SetCharacter(character.Name, playableCharacter);
            CharactersGrid.AddChild(icon);
        }
    }

    private void RefreshSlottedStates()
    {
        foreach (var icon in Icons)
        {
            icon.SetSlotted(CharacterFormation.IsSlotted(icon.Character.Unit));
        }
    }
}

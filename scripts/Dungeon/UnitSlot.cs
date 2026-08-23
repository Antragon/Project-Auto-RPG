namespace Game.scripts.Dungeon;

using Extensions;
using Godot;
using Units;

public partial class UnitSlot : Node2D
{
    private CharacterFormationService CharacterFormation => field ??= this.Root.Get<CharacterFormationService>();

    private AnimatedSprite2D UnitSprite => field ??= GetNode<AnimatedSprite2D>("UnitSprite");

    private Sprite2D DropIndicatorSprite => field ??= GetNode<Sprite2D>("DropIndicatorSprite");

    [Export] public int SlotIndex { get; set; }

    [Export] public bool AcceptsCharacters { get; set; }

    public override void _Ready()
    {
        if (AcceptsCharacters)
        {
            AddToGroup("character_drop_indicators");
            CharacterFormation.Changed += Refresh;
        }

        Refresh();
    }

    public override void _ExitTree()
    {
        if (AcceptsCharacters)
        {
            CharacterFormation.Changed -= Refresh;
        }
    }

    public bool CanAcceptCharacter(Variant data)
    {
        return AcceptsCharacters && TryGetCharacterName(data, out _);
    }

    public void SetDropIndicatorVisible(bool visible)
    {
        if (AcceptsCharacters)
        {
            DropIndicatorSprite.Visible = visible;
        }
    }

    public void AcceptCharacter(Variant data)
    {
        if (TryGetCharacterName(data, out var characterName))
        {
            CharacterFormation.Assign(characterName, SlotIndex);
        }
    }

    private void Refresh()
    {
        if (!AcceptsCharacters)
        {
            return;
        }

        var characterName = CharacterFormation.GetCharacter(SlotIndex);
        if (characterName is null)
        {
            UnitSprite.Stop();
            UnitSprite.SpriteFrames = null;
            UnitSprite.Visible = false;
            return;
        }

        var spriteFrames = GD.Load<SpriteFrames>($"res://textures/units/{characterName}.tres");
        if (spriteFrames is null)
        {
            GD.PushWarning($"Could not load SpriteFrames for character '{characterName}'.");
            UnitSprite.Stop();
            UnitSprite.SpriteFrames = null;
            UnitSprite.Visible = false;
            return;
        }

        UnitSprite.SpriteFrames = spriteFrames;
        var spriteSize = spriteFrames.GetFrameTexture("idle", 0).GetSize();
        UnitSprite.Offset = new Vector2(0, -spriteSize.Y / 2f);
        UnitSprite.Scale = Vector2.One * (256f / spriteSize.Y);
        UnitSprite.Visible = true;
        UnitSprite.Play("idle");
    }

    private static bool TryGetCharacterName(Variant data, out string characterName)
    {
        characterName = string.Empty;
        if (data.VariantType != Variant.Type.String)
        {
            return false;
        }

        characterName = data.AsString();
        return !string.IsNullOrWhiteSpace(characterName);
    }
}

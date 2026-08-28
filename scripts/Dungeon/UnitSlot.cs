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

    public Unit? Unit { get; private set; }

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
        return AcceptsCharacters && TryGetUnitData(data, out _);
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
        if (TryGetUnitData(data, out var unitData))
        {
            CharacterFormation.Assign(unitData, SlotIndex);
        }
    }

    private void Refresh()
    {
        if (!AcceptsCharacters)
        {
            return;
        }

        var unitData = CharacterFormation.GetUnitData(SlotIndex);
        Unit = unitData is null ? null : new Unit(unitData);

        if (unitData is null)
        {
            UnitSprite.Stop();
            UnitSprite.SpriteFrames = null;
            UnitSprite.Visible = false;
            return;
        }

        var spriteFrames = UnitSpriteFramesRepository.Load(unitData.Name);
        if (spriteFrames is null)
        {
            GD.PushWarning($"Could not load SpriteFrames for character '{unitData.Name}'.");
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

    private static bool TryGetUnitData(Variant data, out UnitData unitData)
    {
        unitData = null!;
        if (data.VariantType != Variant.Type.Object)
        {
            return false;
        }

        if (data.AsGodotObject() is not UnitData dataUnitData)
        {
            return false;
        }

        unitData = dataUnitData;
        return true;
    }
}

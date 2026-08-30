namespace Game.scripts.Dungeon;

using System;
using Extensions;
using Godot;
using Units;

public partial class UnitSlot : Node2D
{
    private AnimatedSprite2D UnitSprite => field ??= GetNode<AnimatedSprite2D>("UnitSprite");

    private Sprite2D DropIndicatorSprite => field ??= GetNode<Sprite2D>("DropIndicatorSprite");

    private UnitSkillSlots SkillSlots => field ??= GetNode<UnitSkillSlots>("SkillSlots");

    [Export] public int SlotIndex { get; set; }

    [Export] public bool AcceptsCharacters { get; set; }

    public Unit? Unit { get; private set; }

    public event Action<UnitSlot>? Changed;

    public override void _Ready()
    {
        if (AcceptsCharacters)
        {
            AddToGroup("character_drop_indicators");
        }
    }

    public void SetState(DungeonState dungeonState)
    {
        SkillSlots.SetActive(dungeonState == DungeonState.Combat);
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

    public void Assign(Variant data)
    {
        if (TryGetUnitData(data, out var unitData))
        {
            Assign(unitData);
        }
    }

    public void Assign(UnitData unitData)
    {
        Unit = new Unit(unitData);
        RefreshSprite(unitData);
        Changed?.Invoke(this);
    }

    public void PlayAnimation(StringName animationName)
    {
        if (Unit is not null)
        {
            UnitSprite.Play(animationName);
        }
    }

    public void Clear()
    {
        if (Unit is null)
        {
            return;
        }

        Unit = null;
        UnitSprite.Stop();
        UnitSprite.SpriteFrames = null;
        UnitSprite.Visible = false;
        Changed?.Invoke(this);
    }

    private void RefreshSprite(UnitData unitData)
    {
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

namespace Game.scripts.Units;

using Dungeon;
using Extensions;
using Godot;
using Skills;

public partial class UnitAnimation : AnimatedSprite2D
{
    private UnitSlot UnitSlot => field ??= GetParent<UnitSlot>();

    private UnitSkillSlots UnitSkillSlots => field ??= this.GetSibling<UnitSkillSlots>();

    public override void _Ready()
    {
        UnitSlot.PropertyChanged += OnUnitSlotPropertyChanged;
        UnitSkillSlots.SkillTriggered += OnSkillTriggered;
        AnimationFinished += OnAnimationFinished;
    }

    public override void _ExitTree()
    {
        UnitSlot.PropertyChanged -= OnUnitSlotPropertyChanged;
        UnitSkillSlots.SkillTriggered -= OnSkillTriggered;
        AnimationFinished -= OnAnimationFinished;
    }

    private void OnUnitSlotPropertyChanged(UnitSlot sender, string propertyName)
    {
        if (propertyName == nameof(UnitSlot.Unit))
        {
            RefreshSprite(sender.Unit);
        }

        if (propertyName == nameof(UnitSlot.DungeonState))
        {
            OnStateChanged();
        }
    }

    private void OnStateChanged()
    {
        if (SpriteFrames is not null && Visible)
        {
            UpdateAnimation();
        }
    }

    private void OnSkillTriggered(SkillData skillData)
    {
        if (SpriteFrames is null || !Visible)
        {
            return;
        }

        Play("attack");
    }

    private void OnAnimationFinished()
    {
        if (Animation == "attack")
        {
            UpdateAnimation();
        }
    }

    private void RefreshSprite(Unit? unit)
    {
        if (unit is null)
        {
            Stop();
            SpriteFrames = null;
            Visible = false;
            return;
        }

        var spriteFrames = UnitSpriteFramesRepository.Load(unit.UnitData.Name);
        if (spriteFrames is null)
        {
            Stop();
            SpriteFrames = null;
            Visible = false;
            return;
        }

        SpriteFrames = spriteFrames;
        var spriteSize = spriteFrames.GetFrameTexture("idle", 0).GetSize();
        Offset = new Vector2(0, -spriteSize.Y / 2f);
        Scale = Vector2.One * (256f / spriteSize.Y);
        FlipH = UnitSlot.GetParent() is EnemyFormation;
        Visible = true;
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        Stop();

        var animationName = UnitSlot.DungeonState == DungeonState.Walking
            ? "walk"
            : "idle";

        if (Animation == animationName && IsPlaying())
        {
            return;
        }

        Play(animationName);
    }
}

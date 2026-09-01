namespace Game.scripts.Units;

using Dungeon;
using Extensions;
using Godot;

public partial class UnitAnimation : AnimatedSprite2D
{
    private UnitSlot UnitSlot => field ??= GetParent<UnitSlot>();

    private UnitSkillSlots UnitSkillSlots => field ??= this.GetSibling<UnitSkillSlots>();

    private Vector2 _defaultPosition;
    private Tween? _bumpTween;

    public override void _Ready()
    {
        _defaultPosition = Position;
        UnitSlot.StateChanged += OnStateChanged;
        UnitSlot.UnitChanged += OnUnitChanged;
        UnitSkillSlots.SkillTriggered += OnSkillTriggered;
        AnimationFinished += OnAnimationFinished;
    }

    public override void _ExitTree()
    {
        UnitSlot.StateChanged -= OnStateChanged;
        UnitSlot.UnitChanged -= OnUnitChanged;
        UnitSkillSlots.SkillTriggered -= OnSkillTriggered;
        AnimationFinished -= OnAnimationFinished;
        _bumpTween?.Kill();
    }

    private void OnStateChanged()
    {
        if (SpriteFrames is not null && Visible)
        {
            UpdateAnimation();
        }
    }

    private void OnUnitChanged(UnitSlot changedSlot)
    {
        RefreshSprite(changedSlot.Unit);
    }

    private void OnSkillTriggered()
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

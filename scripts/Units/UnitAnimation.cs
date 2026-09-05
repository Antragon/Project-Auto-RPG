namespace Game.scripts.Units;

using Dungeon;
using Extensions;
using Godot;
using Skills;

public partial class UnitAnimation : AnimatedSprite2D
{
    private UnitSlot UnitSlot => field ??= GetParent<UnitSlot>();

    private UnitSkillSlots UnitSkillSlots => field ??= this.GetSibling<UnitSkillSlots>();

    public bool UnitIsDead { get; private set; }

    public override void _Ready()
    {
        AnimationFinished += OnAnimationFinished;
        UnitSkillSlots.SkillTriggered += OnSkillTriggered;
        UnitSlot.PropertyChanged += OnUnitSlotPropertyChanged;
        UnitSlot.UnitPropertyChanged += OnUnitPropertyChanged;
    }

    public override void _ExitTree()
    {
        AnimationFinished -= OnAnimationFinished;
        UnitSkillSlots.SkillTriggered -= OnSkillTriggered;
        UnitSlot.PropertyChanged -= OnUnitSlotPropertyChanged;
        UnitSlot.UnitPropertyChanged -= OnUnitPropertyChanged;
    }

    private void OnAnimationFinished()
    {
        if (Animation == "death")
        {
            UnitIsDead = true;
        }
        else if (Animation == "attack")
        {
            UpdateAnimation();
        }
    }

    private void OnSkillTriggered(SkillData skillData)
    {
        PlayAnimation("attack");
    }

    private void OnUnitSlotPropertyChanged(UnitSlot sender, string propertyName)
    {
        if (propertyName == nameof(UnitSlot.Unit))
        {
            RefreshSprite();
            if (UnitSlot.Unit?.IsDead == true)
            {
                PlayAnimation("death");
            }
            else
            {
                UpdateAnimation();
            }
        }
        else if (propertyName == nameof(UnitSlot.DungeonState) && UnitSlot.Unit?.IsDead == false)
        {
            UpdateAnimation();
        }
    }

    private void OnUnitPropertyChanged(UnitSlot sender, string propertyName)
    {
        if (propertyName == nameof(Unit.IsDead) && !UnitIsDead && sender.Unit?.IsDead == true)
        {
            PlayAnimation("death");
        }
    }

    private void RefreshSprite()
    {
        UnitIsDead = false;

        var unit = UnitSlot.Unit;
        if (unit is null || !UnitSpriteFramesRepository.TryLoad(unit.UnitData.Name, out var spriteFrames))
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

        PlayAnimation(animationName);
    }

    private void PlayAnimation(string animationName)
    {
        UnitIsDead = false;
        if (SpriteFrames is not null && Visible)
        {
            Play(animationName);
        }
    }
}

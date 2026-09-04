namespace Game.scripts.Units;

using Dungeon;
using Extensions;
using Godot;
using Skills;

public partial class UnitAnimation : AnimatedSprite2D
{
    private UnitSlot UnitSlot => field ??= GetParent<UnitSlot>();

    private UnitSkillSlots UnitSkillSlots => field ??= this.GetSibling<UnitSkillSlots>();

    private Unit? _subscribedUnit;

    public bool UnitIsDead { get; private set; }

    public override void _Ready()
    {
        UnitSlot.PropertyChanged += OnUnitSlotPropertyChanged;
        UnitSkillSlots.SkillTriggered += OnSkillTriggered;
        AnimationFinished += OnAnimationFinished;
        SubscribeToUnit(UnitSlot.Unit);
    }

    public override void _ExitTree()
    {
        UnitSlot.PropertyChanged -= OnUnitSlotPropertyChanged;
        UnitSkillSlots.SkillTriggered -= OnSkillTriggered;
        AnimationFinished -= OnAnimationFinished;
        SubscribeToUnit(null);
    }

    private void OnUnitSlotPropertyChanged(UnitSlot sender, string propertyName)
    {
        if (propertyName == nameof(UnitSlot.Unit))
        {
            SubscribeToUnit(sender.Unit);
            RefreshSprite(sender.Unit);
        }

        if (propertyName == nameof(UnitSlot.DungeonState))
        {
            OnStateChanged();
        }
    }

    private void OnStateChanged()
    {
        if (UnitSlot.Unit is { IsDead: true })
        {
            PlayDeath();
            return;
        }

        if (SpriteFrames is not null && Visible)
        {
            UpdateAnimation();
        }
    }

    private void OnSkillTriggered(SkillData skillData)
    {
        if (SpriteFrames is null || !Visible || UnitSlot.Unit is not { IsDead: false })
        {
            return;
        }

        Play("attack");
    }

    private void OnAnimationFinished()
    {
        if (Animation == "death")
        {
            UnitIsDead = true;
            return;
        }

        if (Animation == "attack")
        {
            UpdateAnimation();
        }
    }

    private void SubscribeToUnit(Unit? unit)
    {
        if (_subscribedUnit is not null)
        {
            _subscribedUnit.Changed -= OnUnitChanged;
        }

        _subscribedUnit = unit;
        if (_subscribedUnit is not null)
        {
            _subscribedUnit.Changed += OnUnitChanged;
        }
    }

    private void OnUnitChanged()
    {
        if (_subscribedUnit is { IsDead: true })
        {
            PlayDeath();
        }
    }

    private void RefreshSprite(Unit? unit)
    {
        UnitIsDead = false;

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

        if (unit.IsDead)
        {
            PlayDeath();
        }
        else
        {
            UpdateAnimation();
        }
    }

    private void UpdateAnimation()
    {
        if (UnitSlot.Unit?.IsDead == true)
        {
            return;
        }

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

    private void PlayDeath()
    {
        if (SpriteFrames is null || !Visible || Animation == "death")
        {
            return;
        }

        UnitIsDead = false;
        Play("death");
    }
}

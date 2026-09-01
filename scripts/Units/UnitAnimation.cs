namespace Game.scripts.Units;

using Dungeon;
using Godot;

public partial class UnitAnimation : AnimatedSprite2D
{
    private UnitSlot UnitSlot => field ??= GetParent<UnitSlot>();

    public override void _Ready()
    {
        UnitSlot.StateChanged += OnStateChanged;
        UnitSlot.UnitChanged += OnUnitChanged;
        UpdateAnimation();
    }

    public override void _ExitTree()
    {
        UnitSlot.StateChanged -= OnStateChanged;
        UnitSlot.UnitChanged -= OnUnitChanged;
    }

    private void OnStateChanged()
    {
        UpdateAnimation();
    }

    private void OnUnitChanged(UnitSlot changedSlot)
    {
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (SpriteFrames is null || !Visible)
        {
            Stop();
            return;
        }

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

namespace Game.scripts.Dungeon;

using System;
using Godot;
using Skills;

public partial class SkillSlot : Panel
{
    private const float CooldownInset = 2;

    private const float CooldownDiameter = 24;

    private TextureRect Icon => field ??= GetNode<TextureRect>("Icon");

    private Control CooldownClip => field ??= GetNode<Control>("Cooldown");

    private SkillData? _skillData;

    private float _cooldownRemaining;

    private bool _active;

    public event Action<SkillData>? SkillTriggered;

    public override void _Process(double delta)
    {
        if (!_active || _skillData is null)
        {
            return;
        }

        if (_cooldownRemaining > 0)
        {
            _cooldownRemaining = Mathf.MoveToward(
                _cooldownRemaining,
                0,
                (float)delta);
            RefreshCooldown();
        }

        if (_cooldownRemaining <= 0)
        {
            UseSkill();
        }
    }

    public void SetSkill(SkillData? skillData)
    {
        _skillData = skillData;
        ResetCooldown();
        Icon.Texture = _skillData?.Icon;
    }

    public void SetActive(bool active)
    {
        _active = active;
        if (!active)
        {
            ResetCooldown();
        }
    }

    private void UseSkill()
    {
        _cooldownRemaining = Mathf.Max(0, _skillData!.Cooldown);
        RefreshCooldown();
        SkillTriggered?.Invoke(_skillData);
    }

    private void ResetCooldown()
    {
        _cooldownRemaining = _skillData?.Cooldown ?? 0;
        RefreshCooldown();
    }

    private void RefreshCooldown()
    {
        var cooldown = _skillData?.Cooldown ?? 0;
        var remainingRatio = cooldown > 0
            ? Mathf.Clamp(_cooldownRemaining / cooldown, 0, 1)
            : 0;

        CooldownClip.Visible = remainingRatio > 0;
        CooldownClip.OffsetTop = -CooldownInset - CooldownDiameter * remainingRatio;
    }
}

namespace Game.scripts.Dungeon;

using System;
using System.Collections.Generic;
using Extensions;
using Godot;
using Loot;
using Units;

public partial class DungeonLoot : Node2D
{
    private LootRepository LootRepository => field ??= this.Root.Get<LootRepository>();

    private EnemyFormation EnemyFormation => field ??= this.GetSibling<EnemyFormation>();

    private readonly List<(UnitSlot Slot, UnitAnimation Animation, Action Handler)> _subscriptions = [];
    private readonly HashSet<UnitSlot> _awardedSlots = [];
    private readonly Dictionary<UnitSlot, (Vector2 Position, Dictionary<LootData, int> Drops)> _pending = [];

    public override void _Ready()
    {
        foreach (var enemySlot in EnemyFormation.EnemySlots)
        {
            var slot = enemySlot.UnitSlot;
            var animation = slot.GetChildOfType<UnitAnimation>();
            Action handler = () => OnAnimationFinished(slot, animation);
            slot.PropertyChanged += OnSlotPropertyChanged;
            slot.UnitPropertyChanged += OnUnitPropertyChanged;
            animation.AnimationFinished += handler;
            _subscriptions.Add((slot, animation, handler));
        }
    }

    public override void _ExitTree()
    {
        foreach (var (slot, animation, handler) in _subscriptions)
        {
            slot.PropertyChanged -= OnSlotPropertyChanged;
            slot.UnitPropertyChanged -= OnUnitPropertyChanged;
            animation.AnimationFinished -= handler;
        }

        _subscriptions.Clear();
        _awardedSlots.Clear();
        _pending.Clear();
    }

    private void OnSlotPropertyChanged(UnitSlot sender, string propertyName)
    {
        if (propertyName == nameof(UnitSlot.Unit))
        {
            _awardedSlots.Remove(sender);
            _pending.Remove(sender);
        }
    }

    private void OnUnitPropertyChanged(UnitSlot sender, string propertyName)
    {
        if (propertyName != nameof(Unit.IsDead) || sender.Unit?.IsDead != true
            || !_awardedSlots.Add(sender))
        {
            return;
        }

        var enemy = sender.GetParent<EnemySlot>().Enemy;
        if (enemy is null)
        {
            return;
        }

        var drops = new Dictionary<LootData, int>();
        foreach (var lootData in enemy.EnemyData.Loot)
        {
            if (lootData is null)
            {
                continue;
            }

            LootRepository.Add(lootData.Name, 1);
            drops.TryGetValue(lootData, out var amount);
            drops[lootData] = amount + 1;
        }

        if (drops.Count > 0)
        {
            _pending[sender] = (sender.GlobalPosition + Vector2.Up * 96, drops);
        }
    }

    private void OnAnimationFinished(UnitSlot slot, UnitAnimation animation)
    {
        if (animation.Animation != "death" || slot.Unit?.IsDead != true
            || !_pending.Remove(slot, out var pending))
        {
            return;
        }

        var popup = new LootDropPopup { ZIndex = 1 };
        popup.SetDrops(pending.Drops);
        AddChild(popup);
        popup.GlobalPosition = pending.Position;
        popup.Animate();
    }
}

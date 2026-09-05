namespace Game.scripts.Dungeon;

using System;
using System.Collections.Generic;
using Extensions;
using Godot;
using Units;

public partial class CharacterExperience : Node
{
    private const int EnemyDeathXp = 5;

    private CharacterFormation CharacterFormation => field ??= this.Root.Get<CharacterFormation>();

    private CharacterRoster CharacterRoster => field ??= this.Root.Get<CharacterRoster>();

    private EnemyFormation EnemyFormation => field ??= GetParent<Dungeon>().GetChildOfType<EnemyFormation>();

    private readonly Dictionary<UnitSlot, (Unit Unit, Action Handler)> _enemySubscriptions = [];

    public override void _Ready()
    {
        foreach (var enemySlot in EnemyFormation.Slots)
        {
            enemySlot.PropertyChanged += OnEnemySlotPropertyChanged;
            SubscribeToEnemy(enemySlot);
        }
    }

    public override void _ExitTree()
    {
        foreach (var enemySlot in EnemyFormation.Slots)
        {
            enemySlot.PropertyChanged -= OnEnemySlotPropertyChanged;
            UnsubscribeFromEnemy(enemySlot);
        }
    }

    private void OnEnemySlotPropertyChanged(UnitSlot sender, string propertyName)
    {
        if (propertyName == nameof(UnitSlot.Unit))
        {
            SubscribeToEnemy(sender);
        }
    }

    private void SubscribeToEnemy(UnitSlot enemySlot)
    {
        UnsubscribeFromEnemy(enemySlot);

        if (enemySlot.Unit is not { } enemy)
        {
            return;
        }

        var handler = () => OnEnemyHpChanged(enemy);
        _enemySubscriptions.Add(enemySlot, (enemy, handler));
        enemy.HpChanged += handler;
    }

    private void UnsubscribeFromEnemy(UnitSlot enemySlot)
    {
        if (!_enemySubscriptions.Remove(enemySlot, out var subscription))
        {
            return;
        }

        subscription.Unit.HpChanged -= subscription.Handler;
    }

    private void OnEnemyHpChanged(Unit enemy)
    {
        if (!enemy.IsDead)
        {
            return;
        }

        foreach (var characterSlot in CharacterFormation.Slots)
        {
            if (characterSlot.Unit is not { IsDead: false } characterUnit
                || !CharacterRoster.TryGetCharacter(characterUnit.UnitData.Name, out var character))
            {
                continue;
            }

            character.AddXp(EnemyDeathXp);
        }
    }
}

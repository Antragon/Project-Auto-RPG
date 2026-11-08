namespace Game.scripts.Dungeon;

using Extensions;
using Godot;
using Units;

public partial class CharacterExperience : Node
{
    private CharacterFormation CharacterFormation => field ??= this.Root.Get<CharacterFormation>();

    private CharacterSlot[] CharacterSlots => field ??= CharacterFormation.GetChildrenOfType<CharacterSlot>();

    private EnemyFormation EnemyFormation => field ??= GetParent<Dungeon>().GetChildOfType<EnemyFormation>();

    public override void _Ready()
    {
        foreach (var enemySlot in EnemyFormation.Slots)
        {
            enemySlot.UnitPropertyChanged += OnEnemyUnitPropertyChanged;
        }
    }

    public override void _ExitTree()
    {
        foreach (var enemySlot in EnemyFormation.Slots)
        {
            enemySlot.UnitPropertyChanged -= OnEnemyUnitPropertyChanged;
        }
    }

    private void OnEnemyUnitPropertyChanged(UnitSlot sender, string propertyName)
    {
        if (propertyName != nameof(Unit.IsDead) || sender.Unit?.IsDead != true
            || sender.GetParent<EnemySlot>().Enemy is not { } enemy)
        {
            return;
        }

        foreach (var characterSlot in CharacterSlots)
        {
            if (characterSlot.Character is { Unit.IsDead: false } character)
            {
                character.AddXp(enemy.XpReward);
            }
        }
    }
}

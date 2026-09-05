namespace Game.scripts.Dungeon;

using Extensions;
using Godot;
using Units;

public partial class CharacterExperience : Node
{
    private const int EnemyDeathXp = 5;

    private CharacterFormation CharacterFormation => field ??= this.Root.Get<CharacterFormation>();

    private CharacterRoster CharacterRoster => field ??= this.Root.Get<CharacterRoster>();

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
        if (propertyName != nameof(Unit.IsDead) || sender.Unit?.IsDead != true)
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

namespace Game.scripts.Dungeon;

using System.Linq;
using Godot;
using Units;

public partial class Dungeon : Node2D
{
    private Node2D EnemyFormation => field ??= GetNode<Node2D>("EnemyFormation");

    private UnitSlot[] EnemySlots => field ??= [.. EnemyFormation.GetChildren().OfType<UnitSlot>()];

    private RandomNumberGenerator Random => field ??= CreateRandomNumberGenerator();

    private DungeonData? _dungeonData;

    public override void _Process(double delta)
    {
        if (_dungeonData is null || EnemySlots.Any(slot => slot.Unit is not null) || _dungeonData.Enemies.Count == 0)
        {
            return;
        }

        var enemyName = _dungeonData.Enemies[Random.RandiRange(0, _dungeonData.Enemies.Count - 1)];
        var enemyData = UnitDataRepository.Load(enemyName);
        if (enemyData is null)
        {
            GD.PushWarning($"Could not load enemy unit '{enemyName}'.");
            return;
        }

        EnemySlots[0].Assign(enemyData);
    }

    public void Enter(string name)
    {
        _dungeonData = DungeonDataRepository.Load(name);
        if (_dungeonData is null)
        {
            GD.PushWarning($"Could not load dungeon '{name}'.");
        }

        foreach (var enemySlot in EnemySlots)
        {
            enemySlot.Clear();
        }
    }

    private static RandomNumberGenerator CreateRandomNumberGenerator()
    {
        var random = new RandomNumberGenerator();
        random.Randomize();
        return random;
    }
}

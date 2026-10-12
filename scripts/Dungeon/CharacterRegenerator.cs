namespace Game.scripts.Dungeon;

using System.Linq;
using Extensions;
using Godot;
using Units;

public partial class CharacterRegenerator : Node
{
    private const double RegenerationRate = 0.1;

    private Dungeon Dungeon => field ??= GetParent<Dungeon>();

    private CharacterRepository CharacterRepository => field ??= this.Root.Get<CharacterRepository>();

    private CharacterFormation CharacterFormation => field ??= this.Root.Get<CharacterFormation>();

    private double _timeSinceLastRegeneration;

    public override void _Process(double delta)
    {
        _timeSinceLastRegeneration += delta;
        if (_timeSinceLastRegeneration < 1)
        {
            return;
        }

        _timeSinceLastRegeneration = 0;
        var unitsToRegenerate = CharacterRepository.Characters.Values
            .Select(character => character.Unit)
            .Where(CanBeRegenerated);

        foreach (var unit in unitsToRegenerate)
        {
            var healing = Mathf.CeilToInt(unit.MaxHp * RegenerationRate);
            unit.SetHp(unit.Hp + healing);
        }
    }

    private bool CanBeRegenerated(Unit unit)
    {
        if (!CharacterFormation.IsSlotted(unit))
        {
            return true;
        }

        if (unit.IsDead && Dungeon.State == DungeonState.Idle)
        {
            return true;
        }

        return false;
    }
}

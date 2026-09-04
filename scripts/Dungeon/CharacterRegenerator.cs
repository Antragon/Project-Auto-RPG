namespace Game.scripts.Dungeon;

using Extensions;
using Godot;
using Units;

public partial class CharacterRegenerator : Node
{
    private const double RegenerationRate = 0.1;

    private Dungeon Dungeon => field ??= GetParent<Dungeon>();

    private CharacterFormation CharacterFormation => field ??= this.GetSibling<CharacterFormation>();

    private double _timeSinceLastRegeneration;

    public override void _Process(double delta)
    {
        if (Dungeon.State != DungeonState.Idle)
        {
            _timeSinceLastRegeneration = 0;
            return;
        }

        _timeSinceLastRegeneration += delta;
        if (_timeSinceLastRegeneration < 1)
        {
            return;
        }

        _timeSinceLastRegeneration = 0;
        foreach (var characterSlot in CharacterFormation.Slots)
        {
            var unit = characterSlot.Unit;
            if (unit is null)
            {
                continue;
            }

            var healing = Mathf.CeilToInt(unit.MaxHp * RegenerationRate);
            unit.SetHp(unit.Hp + healing);
        }
    }
}

namespace Game.scripts.Dungeon;

using Godot;

public partial class DungeonBackground : TileMapLayer
{
    private const int BackgroundSourceId = 0;

    private static readonly Vector2I BackgroundTile = Vector2I.Zero;

    public override void _Ready()
    {
        for (var x = -1; x <= 3; x++)
        {
            SetCell(new Vector2I(x, 0), BackgroundSourceId, BackgroundTile);
        }
    }
}

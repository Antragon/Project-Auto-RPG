namespace Game.scripts.Dungeon;

using Godot;

public partial class DungeonBackground : TileMapLayer
{
    private const int BackgroundSourceId = 0;
    private const float WalkSpeed = 200f;
    private static readonly Vector2I BackgroundTile = Vector2I.Zero;

    private Dungeon Dungeon => field ??= GetParent<Dungeon>();

    private Vector2 _defaultPosition;

    public override void _Ready()
    {
        _defaultPosition = Position;

        for (var x = -1; x <= 3; x++)
        {
            SetCell(new Vector2I(x, 0), BackgroundSourceId, BackgroundTile);
        }
    }

    public override void _Process(double delta)
    {
        if (Dungeon.State == DungeonState.Walking)
        {
            UpdateWalking(delta);
        }
    }

    private void UpdateWalking(double delta)
    {
        var nextX = Position.X - WalkSpeed * (float)delta;
        var tileWidth = TileSet.TileSize.X * Scale.X;

        if (nextX <= _defaultPosition.X - tileWidth)
        {
            nextX += tileWidth;
        }

        Position = new Vector2(nextX, _defaultPosition.Y);
    }
}

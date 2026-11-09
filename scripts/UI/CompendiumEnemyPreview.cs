namespace Game.scripts.UI;

using Extensions;
using Godot;
using Units;

public partial class CompendiumEnemyPreview : Control
{
    private AnimatedSprite2D Sprite => field ??= this.GetChildOfType<AnimatedSprite2D>();

    private Label Message => field ??= this.GetChildOfType<Label>();

    public override void _Ready() => Resized += FitPreview;

    public override void _ExitTree() => Resized -= FitPreview;

    public void Stop() => Sprite.Stop();

    public void ShowEnemy(EnemyData enemy, bool encountered)
    {
        Sprite.Stop();
        Sprite.SpriteFrames = null;
        Sprite.Hide();
        Message.Text = encountered ? "Idle animation unavailable" : "?";
        Message.Show();
        var frames = encountered ? UnitSpriteFramesRepository.Load(enemy.Name) : null;
        if (frames is null || !frames.HasAnimation("idle") || frames.GetFrameCount("idle") == 0) return;

        Sprite.SpriteFrames = frames;
        Sprite.Show();
        Message.Hide();
        Sprite.Play("idle");
        FitPreview();
    }

    private void FitPreview()
    {
        if (Sprite.SpriteFrames is not { } frames) return;

        var size = Vector2.One;
        for (var i = 0; i < frames.GetFrameCount("idle"); i++)
        {
            if (frames.GetFrameTexture("idle", i) is { } texture)
            {
                var frameSize = texture.GetSize();
                size = new Vector2(Mathf.Max(size.X, frameSize.X), Mathf.Max(size.Y, frameSize.Y));
            }
        }

        Sprite.Position = Size / 2;
        var available = (Size - new Vector2(24, 24)).Max(Vector2.One);
        Sprite.Scale = Vector2.One * Mathf.Min(available.X / size.X, available.Y / size.Y);
    }
}

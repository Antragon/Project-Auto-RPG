namespace Game.scripts.Dungeon;

using Godot;
using Loot;

public partial class LootDropPopup : Node2D
{
    public void SetRewards(LootData? loot, int xp)
    {
        var rows = new VBoxContainer { MouseFilter = Control.MouseFilterEnum.Ignore };
        if (loot is not null)
        {
            var row = new HBoxContainer
            {
                Alignment = BoxContainer.AlignmentMode.Center,
                MouseFilter = Control.MouseFilterEnum.Ignore,
            };
            row.AddChild(CreateLabel("1x"));
            row.AddChild(new TextureRect
            {
                Texture = loot.Icon,
                CustomMinimumSize = new Vector2(24, 24),
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                MouseFilter = Control.MouseFilterEnum.Ignore,
            });
            rows.AddChild(row);
        }

        rows.AddChild(CreateLabel($"+{xp} XP"));
        rows.Resized += () => rows.Position = -rows.Size / 2;
        AddChild(rows);
    }

    private static Label CreateLabel(string text)
    {
        var label = new Label
        {
            Text = text,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        label.AddThemeFontSizeOverride("font_size", 16);
        label.AddThemeColorOverride("font_outline_color", new Color(0, 0, 0, 0.8f));
        label.AddThemeConstantOverride("outline_size", 3);
        return label;
    }

    public void Animate()
    {
        var tween = CreateTween();
        tween.SetParallel();
        tween.TweenProperty(this, "position", Position + Vector2.Up * 32, 0.8);
        tween.TweenProperty(this, "modulate:a", 0f, 0.8).SetDelay(0.15);
        tween.Chain().TweenCallback(Callable.From(QueueFree));
    }
}

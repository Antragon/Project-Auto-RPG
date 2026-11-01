namespace Game.scripts.Dungeon;

using System.Collections.Generic;
using Godot;
using Loot;

public partial class LootDropPopup : Node2D
{
    public void SetDrops(IReadOnlyDictionary<LootData, int> drops)
    {
        var rows = new VBoxContainer { MouseFilter = Control.MouseFilterEnum.Ignore };
        foreach (var (lootData, amount) in drops)
        {
            var row = new HBoxContainer
            {
                Alignment = BoxContainer.AlignmentMode.Center,
                MouseFilter = Control.MouseFilterEnum.Ignore,
            };
            var label = new Label
            {
                Text = $"{amount}x",
                VerticalAlignment = VerticalAlignment.Center,
                MouseFilter = Control.MouseFilterEnum.Ignore,
            };
            label.AddThemeFontSizeOverride("font_size", 16);
            label.AddThemeColorOverride("font_outline_color", new Color(0, 0, 0, 0.8f));
            label.AddThemeConstantOverride("outline_size", 3);
            row.AddChild(label);
            row.AddChild(new TextureRect
            {
                Texture = lootData.Icon,
                CustomMinimumSize = new Vector2(24, 24),
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                MouseFilter = Control.MouseFilterEnum.Ignore,
            });
            rows.AddChild(row);
        }

        rows.Resized += () => rows.Position = -rows.Size / 2;
        AddChild(rows);
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

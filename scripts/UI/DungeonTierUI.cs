namespace Game.scripts.UI;

using System.Collections.Generic;
using Dungeon;
using Extensions;
using Godot;
using Persistence;

public partial class DungeonTierUI : HBoxContainer
{
    private Dungeon Dungeon => field ??= this.GetAncestor<Dungeon>();

    private StyleBoxFlat NormalStyle => field ??= CreateStyle(new Color(0.12f, 0.15f, 0.21f), new Color(0.42f, 0.48f, 0.58f));

    private StyleBoxFlat HoverStyle => field ??= CreateStyle(new Color(0.2f, 0.25f, 0.34f), new Color(0.65f, 0.72f, 0.82f));

    private StyleBoxFlat SelectedStyle => field ??= CreateStyle(new Color(0.26f, 0.38f, 0.48f), new Color(0.8f, 0.88f, 0.94f));

    private StyleBoxFlat LockedStyle => field ??= CreateStyle(new Color(0.1f, 0.11f, 0.14f), new Color(0.23f, 0.25f, 0.3f));

    private readonly List<Button> _buttons = [];

    private Label? _progressLabel;

    public override void _Ready()
    {
        Dungeon.TiersChanged += Refresh;
        Dungeon.StateChanged += OnStateChanged;
        Refresh();
    }

    public override void _ExitTree()
    {
        Dungeon.TiersChanged -= Refresh;
        Dungeon.StateChanged -= OnStateChanged;
    }

    private void OnStateChanged(DungeonState state) => Refresh();

    private void Refresh()
    {
        var count = Dungeon.Data?.Tiers.Count ?? 0;
        if (_buttons.Count != count)
        {
            foreach (var child in GetChildren())
            {
                RemoveChild(child);
                child.QueueFree();
            }

            _buttons.Clear();
            _progressLabel = null;
            for (var tier = 1; tier <= count; tier++)
            {
                var tierNumber = tier;
                var button = new Button
                {
                    Text = tier.ToString(),
                    CustomMinimumSize = new Vector2(48, 48),
                    MouseDefaultCursorShape = CursorShape.PointingHand,
                };
                button.AddThemeFontSizeOverride("font_size", 20);
                button.Pressed += () => Dungeon.SelectTier(tierNumber);
                AddChild(button);
                _buttons.Add(button);
            }

            if (count > 0)
            {
                _progressLabel = new Label { SizeFlagsVertical = SizeFlags.ShrinkCenter };
                _progressLabel.AddThemeFontSizeOverride("font_size", 18);
                AddChild(_progressLabel);
            }
        }

        if (Dungeon.Progress is not { } progress) return;
        var unlockedCount = 0;
        while (unlockedCount < count && progress.IsUnlocked(unlockedCount + 1, count)) unlockedCount++;

        if (_progressLabel is { } label)
        {
            label.Visible = unlockedCount > 0 && unlockedCount < count;
            if (label.Visible)
            {
                label.Text = $"{progress.GetDefeats(unlockedCount)}/{SavedDungeonProgress.DefeatsToUnlock}";
                MoveChild(label, unlockedCount);
            }
        }

        for (var index = 0; index < count; index++)
        {
            var tier = index + 1;
            var button = _buttons[index];
            var unlocked = progress.IsUnlocked(tier, count);
            var selected = Dungeon.SelectedTier == tier;
            button.Visible = tier <= unlockedCount + 1;
            button.Disabled = !unlocked;
            button.AddThemeStyleboxOverride("normal", selected ? SelectedStyle : NormalStyle);
            button.AddThemeStyleboxOverride("hover", selected ? SelectedStyle : HoverStyle);
            button.AddThemeStyleboxOverride("pressed", SelectedStyle);
            button.AddThemeStyleboxOverride("disabled", LockedStyle);
            button.TooltipText = !unlocked
                ? $"Win {SavedDungeonProgress.DefeatsToUnlock} encounters in tier {tier - 1} ({progress.GetDefeats(tier - 1)}/{SavedDungeonProgress.DefeatsToUnlock})"
                : $"Tier {tier}";
        }
    }

    private static StyleBoxFlat CreateStyle(Color background, Color border)
    {
        return new StyleBoxFlat
        {
            BgColor = background,
            BorderColor = border,
            BorderWidthLeft = 2,
            BorderWidthTop = 2,
            BorderWidthRight = 2,
            BorderWidthBottom = 2,
            CornerRadiusTopLeft = 24,
            CornerRadiusTopRight = 24,
            CornerRadiusBottomRight = 24,
            CornerRadiusBottomLeft = 24,
        };
    }
}

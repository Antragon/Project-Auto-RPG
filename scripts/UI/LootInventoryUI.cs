namespace Game.scripts.UI;

using System.Linq;
using Extensions;
using Godot;
using Loot;

public partial class LootInventoryUI : Control
{
    private LootRepository LootRepository => field ??= this.Root.Get<LootRepository>();

    private Button BackpackButton => field ??= this.GetChildOfType<Button>();

    private Panel LootPanel => field ??= this.GetChildOfType<Panel>();

    private Button CloseButton => field ??= LootPanel.GetChildOfType<Button>();

    private VBoxContainer LootList => field ??= LootPanel.GetChildOfType<ScrollContainer>().GetChildOfType<VBoxContainer>();

    public override void _Ready()
    {
        BackpackButton.Toggled += OnBackpackToggled;
        CloseButton.Pressed += OnClosePressed;
        LootRepository.Changed += RefreshLoot;
    }

    public override void _ExitTree()
    {
        BackpackButton.Toggled -= OnBackpackToggled;
        CloseButton.Pressed -= OnClosePressed;
        LootRepository.Changed -= RefreshLoot;
    }

    private void OnBackpackToggled(bool pressed)
    {
        LootPanel.Visible = pressed;
        RefreshLoot();
    }

    private void OnClosePressed()
    {
        BackpackButton.ButtonPressed = false;
    }

    private void RefreshLoot()
    {
        if (!LootPanel.Visible)
        {
            return;
        }

        foreach (var child in LootList.GetChildren())
        {
            LootList.RemoveChild(child);
            child.QueueFree();
        }

        foreach (var loot in LootRepository.Loot.Where(loot => loot.Amount > 0).OrderBy(loot => loot.Name))
        {
            var row = new HBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
            row.AddThemeConstantOverride("separation", 12);
            row.AddChild(new TextureRect
            {
                Texture = LootDataRepository.Load(loot.Name)?.Icon,
                CustomMinimumSize = new Vector2(32, 32),
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                MouseFilter = MouseFilterEnum.Ignore,
            });
            row.AddChild(new Label
            {
                Text = loot.Name,
                SizeFlagsHorizontal = SizeFlags.ExpandFill,
                TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis,
                TooltipText = loot.Name,
            });
            row.AddChild(new Label
            {
                Text = loot.Amount.ToString(),
                HorizontalAlignment = HorizontalAlignment.Right,
                MouseFilter = MouseFilterEnum.Ignore,
            });
            LootList.AddChild(row);
        }
    }
}

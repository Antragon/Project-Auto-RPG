namespace Game.scripts.UI;

using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Godot;
using Skills;
using Units;

public partial class CompendiumUI : CanvasLayer
{
    private EnemyDefeatRepository EnemyDefeatRepository => field ??= this.Root.Get<EnemyDefeatRepository>();

    private EnemyScalingService EnemyScalingService => field ??= this.Root.Get<EnemyScalingService>();

    private Button OpenButton => field ??= this.GetChildOfType<Button>();

    private Panel Overlay => field ??= this.GetChildOfType<Panel>();

    private Button CloseButton => field ??= Overlay.GetNode<Button>("CloseButton");

    private ItemList EnemyList => field ??= Overlay.GetNode<ItemList>("Margin/Columns/Enemies/List");

    private ScrollContainer DetailsScroll => field ??= Overlay.GetNode<ScrollContainer>("Margin/Columns/DetailsScroll");

    private VBoxContainer Details => field ??= DetailsScroll.GetChildOfType<VBoxContainer>();

    private Label EnemyName => field ??= Details.GetNode<Label>("EnemyName");

    private VBoxContainer Bonuses => field ??= Details.GetNode<VBoxContainer>("Summary/BonusesBlock");

    private Label DefeatCount => field ??= Bonuses.GetNode<Label>("DefeatCount");

    private Label ScalingBonus => field ??= Bonuses.GetNode<Label>("ScalingBonus");

    private Label NextBonus => field ??= Bonuses.GetNode<Label>("Next");

    private HBoxContainer Milestones => field ??= Bonuses.GetChildOfType<HBoxContainer>();

    private Label LastMilestone => field ??= Milestones.GetNode<Label>("LastMilestone");

    private Label TargetMilestone => field ??= Milestones.GetNode<Label>("TargetMilestone");

    private ProgressBar MilestoneProgress => field ??= Milestones.GetChildOfType<ProgressBar>();

    private Control Preview => field ??= Details.GetNode<Control>("Summary/Preview");

    private AnimatedSprite2D Sprite => field ??= Preview.GetChildOfType<AnimatedSprite2D>();

    private Label PreviewMessage => field ??= Preview.GetChildOfType<Label>();

    private GridContainer Stats => field ??= Details.GetNode<GridContainer>("Summary/StatsBlock/Stats");

    private GridContainer Skills => field ??= Details.GetNode<GridContainer>("Skills");

    private PackedScene SkillDetailsPanelScene => field ??= SceneRepository.Load("ui/SkillDetailsPanel")!;

    private readonly List<EnemyData> _enemies = [];

    private int _selectedIndex = -1;

    public override void _Ready()
    {
        OpenButton.Pressed += Open;
        CloseButton.Pressed += Close;
        EnemyList.ItemSelected += OnEnemySelected;
        Preview.Resized += FitPreview;
        GetViewport().GuiFocusChanged += OnGuiFocusChanged;
        EnemyDefeatRepository.Changed += RefreshDefeatCount;
        PopulateEnemies();
    }

    public override void _ExitTree()
    {
        OpenButton.Pressed -= Open;
        CloseButton.Pressed -= Close;
        EnemyList.ItemSelected -= OnEnemySelected;
        Preview.Resized -= FitPreview;
        GetViewport().GuiFocusChanged -= OnGuiFocusChanged;
        EnemyDefeatRepository.Changed -= RefreshDefeatCount;
    }

    public override void _Input(InputEvent @event)
    {
        if (Overlay.Visible && @event.IsActionPressed("ui_cancel"))
        {
            Close();
            GetViewport().SetInputAsHandled();
        }
    }

    public void Open()
    {
        OpenButton.Hide();
        Overlay.Show();
        if (_selectedIndex >= 0)
        {
            ShowEnemy(_selectedIndex);
            EnemyList.EnsureCurrentIsVisible();
        }

        EnemyList.GrabFocus();
    }

    public void Close()
    {
        Sprite.Stop();
        Overlay.Hide();
        OpenButton.Show();
        OpenButton.GrabFocus();
    }

    private void OnGuiFocusChanged(Control control)
    {
        if (Overlay.Visible && control != Overlay && !Overlay.IsAncestorOf(control))
        {
            EnemyList.GrabFocus();
        }
    }

    private void PopulateEnemies()
    {
        foreach (var enemy in DirAccess.GetResourceFilesRecursivelyAt("res://resources/enemies")
                     .Select(GD.Load<EnemyData>)
                     .OrderBy(enemy => enemy.Name, StringComparer.OrdinalIgnoreCase))
        {
            _enemies.Add(enemy);
            EnemyList.AddItem(enemy.Name, IconRepository.GetUnit(enemy.Name));
        }

        if (_enemies.Count > 0)
        {
            _selectedIndex = 0;
            EnemyList.Select(0);
        }
        else
        {
            EnemyName.Text = "No enemies available";
            PreviewMessage.Text = "No enemy resources were found.";
        }
    }

    private void OnEnemySelected(long index)
    {
        _selectedIndex = (int)index;
        DetailsScroll.ScrollVertical = 0;
        ShowEnemy(_selectedIndex);
    }

    private void ShowEnemy(int index)
    {
        Sprite.Stop();
        Sprite.SpriteFrames = null;
        Sprite.Hide();
        PreviewMessage.Text = "Idle animation unavailable";
        PreviewMessage.Show();
        ClearChildren(Stats);
        ClearChildren(Skills);

        var enemy = _enemies[index];
        EnemyName.Text = enemy.Name;
        RefreshDefeatCount();
        var frames = UnitSpriteFramesRepository.Load(enemy.Name);
        if (frames is not null && frames.HasAnimation("idle") && frames.GetFrameCount("idle") > 0)
        {
            Sprite.SpriteFrames = frames;
            Sprite.Show();
            PreviewMessage.Hide();
            Sprite.Play("idle");
            FitPreview();
        }

        var unitData = UnitDataRepository.Load(enemy.Name);
        if (unitData is null)
        {
            Stats.AddChild(CreateLabel("Base stats unavailable"));
        }
        else
        {
            AddStat("Max HP", new Unit(unitData).MaxHp);
            AddStat("Vitality", unitData.Vitality);
            AddStat("Strength", unitData.Strength);
            AddStat("Dexterity", unitData.Dexterity);
            AddStat("Magic", unitData.Magic);
            AddStat("Speed", unitData.Speed);
        }

        SkillData?[] skills = [enemy.Skill1 ?? unitData?.BaseSkill, enemy.Skill2, enemy.Skill3, enemy.Skill4];
        foreach (var skill in skills.OfType<SkillData>())
        {
            AddSkill(skill, skill == unitData?.BaseSkill);
        }

        if (Skills.GetChildCount() == 0)
        {
            Skills.AddChild(CreateLabel("No skills available"));
        }
        else if (Skills.GetChildCount() % 2 != 0)
        {
            // Reserve the second column even when only one skill is present.
            Skills.AddChild(new Control
            {
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                MouseFilter = Control.MouseFilterEnum.Ignore,
            });
        }
    }

    private void RefreshDefeatCount()
    {
        if (_selectedIndex >= 0)
        {
            var enemy = _enemies[_selectedIndex];
            var progress = EnemyScalingService.GetProgress(enemy);
            var count = EnemyDefeatRepository.GetCount(enemy.Name);
            var last = progress.LastThreshold;
            DefeatCount.Text = $"Defeated: {count}";
            ScalingBonus.Text = $"All: +{progress.Bonus} {enemy.ScalingStat}";
            LastMilestone.Text = last.ToString();
            if (progress.NextThreshold is { } next)
            {
                NextBonus.Text = "Next:";
                TargetMilestone.Text = next.ToString();
                MilestoneProgress.Value = (double)(count - last) / (next - last);
            }
            else
            {
                NextBonus.Text = "Next: Complete";
                TargetMilestone.Text = "—";
                MilestoneProgress.Value = 1;
            }
        }
    }

    private void FitPreview()
    {
        if (Sprite.SpriteFrames is not { } frames)
        {
            return;
        }

        var size = Vector2.One;
        for (var i = 0; i < frames.GetFrameCount("idle"); i++)
        {
            if (frames.GetFrameTexture("idle", i) is { } texture)
            {
                var frameSize = texture.GetSize();
                size = new Vector2(Mathf.Max(size.X, frameSize.X), Mathf.Max(size.Y, frameSize.Y));
            }
        }

        Sprite.Position = Preview.Size / 2;
        var available = (Preview.Size - new Vector2(24, 24)).Max(Vector2.One);
        Sprite.Scale = Vector2.One * Mathf.Min(available.X / size.X, available.Y / size.Y);
    }

    private void AddStat(string name, int value)
    {
        Stats.AddChild(CreateLabel(name));
        Stats.AddChild(CreateLabel(value.ToString()));
    }

    private void AddSkill(SkillData skill, bool baseSkill)
    {
        var row = new HBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        row.AddThemeConstantOverride("separation", 12);
        var icon = new TextureRect
        {
            Texture = skill.Icon,
            CustomMinimumSize = new Vector2(48, 48),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        row.AddChild(icon);
        var name = skill.ResourcePath.Contains("::") || string.IsNullOrWhiteSpace(skill.Name)
            ? (baseSkill ? "Base attack" : "Skill")
            : skill.Name;
        var panel = SkillDetailsPanelScene.Instantiate<SkillDetailsPanel>();
        panel.ShowCloseButton = false;
        panel.IsEnemySkill = true;
        panel.CustomMinimumSize = new Vector2(304, 170);
        panel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        row.AddChild(panel);
        Skills.AddChild(row);
        panel.ShowSkill(skill, name);
    }

    private static Label CreateLabel(string text, int fontSize = 18)
    {
        var label = new Label
        {
            Text = text,
            AutowrapMode = TextServer.AutowrapMode.WordSmart,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        label.AddThemeFontSizeOverride("font_size", fontSize);
        return label;
    }

    private static void ClearChildren(Node parent)
    {
        foreach (var child in parent.GetChildren())
        {
            parent.RemoveChild(child);
            child.QueueFree();
        }
    }
}

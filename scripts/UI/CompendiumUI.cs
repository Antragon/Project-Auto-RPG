namespace Game.scripts.UI;

using Dungeon;
using Extensions;
using Godot;
using Units;
using DungeonNode = Dungeon.Dungeon;

public partial class CompendiumUI : CanvasLayer
{
    private EnemyDefeatRepository EnemyDefeatRepository => field ??= this.Root.Get<EnemyDefeatRepository>();

    private DungeonNode Dungeon => field ??= this.GetAncestor<DungeonNode>();

    private Button OpenButton => field ??= this.GetChildOfType<Button>();

    private Panel Overlay => field ??= this.GetChildOfType<Panel>();

    private Button CloseButton => field ??= Overlay.GetNode<Button>("CloseButton");

    private CompendiumEnemyList EnemyList => field ??= Overlay.GetNode<CompendiumEnemyList>("Margin/Columns/Enemies/List");

    private ScrollContainer DetailsScroll => field ??= Overlay.GetNode<ScrollContainer>("Margin/Columns/DetailsScroll");

    private VBoxContainer Details => field ??= DetailsScroll.GetChildOfType<VBoxContainer>();

    private Label EnemyName => field ??= Details.GetNode<Label>("EnemyName");

    private HBoxContainer Summary => field ??= Details.GetNode<HBoxContainer>("Summary");

    private CompendiumEnemyPreview Preview => field ??= Summary.GetChildOfType<CompendiumEnemyPreview>();

    private CompendiumEnemyStats Stats => field ??= Summary.GetNode<VBoxContainer>("StatsBlock").GetChildOfType<CompendiumEnemyStats>();

    private CompendiumEnemyBonuses Bonuses => field ??= Summary.GetChildOfType<CompendiumEnemyBonuses>();

    private Label SkillsHeading => field ??= Details.GetNode<Label>("SkillsHeading");

    private CompendiumEnemySkills Skills => field ??= Details.GetChildOfType<CompendiumEnemySkills>();

    public override void _Ready()
    {
        OpenButton.Pressed += Open;
        CloseButton.Pressed += Close;
        EnemyList.SelectionChanged += OnEnemySelected;
        EnemyList.ItemSelected += OnListItemSelected;
        GetViewport().GuiFocusChanged += OnGuiFocusChanged;
        EnemyDefeatRepository.Changed += OnDefeatsChanged;
        Dungeon.StateChanged += OnDungeonStateChanged;
        EnemyList.Refresh();
    }

    public override void _ExitTree()
    {
        OpenButton.Pressed -= Open;
        CloseButton.Pressed -= Close;
        EnemyList.SelectionChanged -= OnEnemySelected;
        EnemyList.ItemSelected -= OnListItemSelected;
        GetViewport().GuiFocusChanged -= OnGuiFocusChanged;
        EnemyDefeatRepository.Changed -= OnDefeatsChanged;
        Dungeon.StateChanged -= OnDungeonStateChanged;
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
        EnemyList.Refresh();
        if (EnemyList.SelectedEnemy is not null) EnemyList.EnsureCurrentIsVisible();
        EnemyList.GrabFocus();
    }

    public void Close()
    {
        Preview.Stop();
        Overlay.Hide();
        OpenButton.Show();
        OpenButton.GrabFocus();
    }

    private void OnGuiFocusChanged(Control control)
    {
        if (Overlay.Visible && control != Overlay && !Overlay.IsAncestorOf(control))
            EnemyList.GrabFocus();
    }

    private void OnEnemySelected(CompendiumEnemy? selection) => ShowEnemy(selection);

    private void OnListItemSelected(long index) => DetailsScroll.ScrollVertical = 0;

    private void ShowEnemy(CompendiumEnemy? selection)
    {
        var hasSelection = selection is not null;
        Summary.Visible = hasSelection;
        SkillsHeading.Visible = hasSelection;
        Skills.Visible = hasSelection;
        if (selection is not { } entry)
        {
            Preview.Stop();
            EnemyName.Text = "No enemies encountered yet";
            return;
        }

        var count = entry.Encountered ? EnemyDefeatRepository.GetCount(entry.Enemy.Name) : 0;
        var unitData = UnitDataRepository.Load(entry.Enemy.Name);
        EnemyName.Text = entry.Encountered ? entry.Enemy.Name : "???";
        Preview.ShowEnemy(entry.Enemy, entry.Encountered);
        Stats.ShowEnemy(unitData, count);
        Skills.ShowEnemy(entry.Enemy, unitData, count);
        Bonuses.ShowEnemy(entry.Enemy, entry.Encountered, count);
    }

    private void OnDefeatsChanged() => ShowEnemy(EnemyList.SelectedEnemy);

    private void OnDungeonStateChanged(DungeonState state)
    {
        if (state == DungeonState.Combat && Overlay.Visible) EnemyList.Refresh();
    }
}

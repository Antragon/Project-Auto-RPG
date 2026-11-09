namespace Game.scripts.UI;

using System;
using System.Collections.Generic;
using System.Linq;
using Dungeon;
using Extensions;
using Godot;
using Persistence;
using Units;

public readonly record struct CompendiumEnemy(string Dungeon, EnemyData Enemy, bool Encountered);

public partial class CompendiumEnemyList : ItemList
{
    private SaveRepository SaveRepository => field ??= this.Root.Get<SaveRepository>();

    private readonly Dictionary<int, CompendiumEnemy> _enemies = [];

    public event Action<CompendiumEnemy?>? SelectionChanged;

    public CompendiumEnemy? SelectedEnemy { get; private set; }

    public override void _Ready() => ItemSelected += OnItemSelected;

    public override void _ExitTree() => ItemSelected -= OnItemSelected;

    public void Refresh()
    {
        var selected = SelectedEnemy;
        _enemies.Clear();
        Clear();
        var selectedIndex = -1;
        var firstEncounteredIndex = -1;
        var unknownIcon = GD.Load<Texture2D>("res://textures/ui/unknown_enemy.svg");
        foreach (var file in DirAccess.GetFilesAt("res://resources/dungeons")
                     .Where(file => file.EndsWith(".tres", StringComparison.OrdinalIgnoreCase))
                     .OrderBy(file => file, StringComparer.OrdinalIgnoreCase))
        {
            var dungeonName = System.IO.Path.GetFileNameWithoutExtension(file);
            if (!SaveRepository.SaveData.Progress.DungeonProgress.TryGetValue(dungeonName, out var progress)
                || progress.EncounteredEnemies is not { Count: > 0 } encountered
                || DungeonDataRepository.Load(dungeonName) is not { } dungeon) continue;

            var enemies = dungeon.Tiers.SelectMany(tier => tier.Enemies)
                .DistinctBy(enemy => enemy.Name)
                .OrderBy(enemy => enemy.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            if (!enemies.Any(enemy => encountered.Contains(enemy.Name))) continue;

            var headingIndex = ItemCount;
            AddItem(dungeonName.Replace('_', ' ').Capitalize());
            SetItemSelectable(headingIndex, false);
            foreach (var enemy in enemies)
            {
                var index = ItemCount;
                var known = encountered.Contains(enemy.Name);
                AddItem(known ? enemy.Name : "???", known ? IconRepository.GetUnit(enemy.Name) : unknownIcon);
                _enemies[index] = new CompendiumEnemy(dungeonName, enemy, known);
                if (known && firstEncounteredIndex < 0) firstEncounteredIndex = index;
                if (selected is { } previous && previous.Dungeon == dungeonName && previous.Enemy.Name == enemy.Name)
                    selectedIndex = index;
            }
        }

        if (selectedIndex < 0) selectedIndex = firstEncounteredIndex;
        if (selectedIndex >= 0)
        {
            Select(selectedIndex);
            SelectedEnemy = _enemies[selectedIndex];
        }
        else
        {
            SelectedEnemy = null;
        }

        SelectionChanged?.Invoke(SelectedEnemy);
    }

    private void OnItemSelected(long index)
    {
        SelectedEnemy = _enemies[(int)index];
        SelectionChanged?.Invoke(SelectedEnemy);
    }
}

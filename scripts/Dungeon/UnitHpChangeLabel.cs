namespace Game.scripts.Dungeon;

using Godot;
using Units;

public partial class UnitHpChangeLabel : Label
{
    private static readonly Color HealingColor = new("70e082");
    private static readonly Color DamageColor = new("f16b6b");

    private UnitSlot UnitSlot => field ??= GetParent<UnitSlot>();

    private Tween? _tween;
    private int _previousHp;
    private Vector2 _initialPosition;

    public override void _Ready()
    {
        _initialPosition = Position;
        UnitSlot.PropertyChanged += OnUnitSlotPropertyChanged;
        UnitSlot.UnitPropertyChanged += OnUnitPropertyChanged;
        RefreshUnit();
    }

    public override void _ExitTree()
    {
        UnitSlot.PropertyChanged -= OnUnitSlotPropertyChanged;
        UnitSlot.UnitPropertyChanged -= OnUnitPropertyChanged;
        _tween?.Kill();
    }

    private void OnUnitSlotPropertyChanged(UnitSlot sender, string propertyName)
    {
        if (propertyName == nameof(UnitSlot.Unit))
        {
            RefreshUnit();
        }
    }

    private void OnUnitPropertyChanged(UnitSlot sender, string propertyName)
    {
        if (propertyName != nameof(Unit.Hp) || sender.Unit is null)
        {
            return;
        }

        var hpChange = sender.Unit.Hp - _previousHp;
        _previousHp = sender.Unit.Hp;

        if (hpChange == 0)
        {
            return;
        }

        ShowChange(hpChange);
    }

    private void RefreshUnit()
    {
        _previousHp = UnitSlot.Unit?.Hp ?? 0;
        Hide();
    }

    private void ShowChange(int hpChange)
    {
        _tween?.Kill();
        Position = _initialPosition;
        Modulate = Colors.White;
        Text = hpChange > 0 ? $"+{hpChange}" : hpChange.ToString();
        AddThemeColorOverride("font_color", hpChange > 0 ? HealingColor : DamageColor);
        Show();

        _tween = CreateTween();
        _tween.SetParallel();
        _tween.TweenProperty(this, "position", _initialPosition + Vector2.Up * 32, 0.8);
        _tween.TweenProperty(this, "modulate:a", 0f, 0.8).SetDelay(0.15);
        _tween.Chain().TweenCallback(Callable.From(Hide));
    }
}

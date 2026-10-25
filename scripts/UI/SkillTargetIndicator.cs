namespace Game.scripts.UI;

using Godot;
using Skills;

public partial class SkillTargetIndicator : Panel
{
    private Label QuestionMark => field ??= GetNode<Label>("QuestionMark");

    private StyleBoxFlat InactiveStyle => field ??= CreateStyle(new Color(0.32f, 0.34f, 0.38f));

    private StyleBoxFlat DamageStyle => field ??= CreateStyle(new Color(0.85f, 0.12f, 0.12f));

    private StyleBoxFlat HealStyle => field ??= CreateStyle(new Color(0.2f, 0.7f, 0.42f));

    public override void _Ready()
    {
        Clear();
    }

    public void SetEffect(SkillEffect effect, bool isRandom)
    {
        var style = effect switch
        {
            SkillEffect.Damage => DamageStyle,
            SkillEffect.Heal => HealStyle,
            _ => InactiveStyle,
        };

        AddThemeStyleboxOverride("panel", style);
        QuestionMark.Visible = isRandom;
    }

    public void Clear()
    {
        AddThemeStyleboxOverride("panel", InactiveStyle);
        QuestionMark.Visible = false;
    }

    private static StyleBoxFlat CreateStyle(Color color)
    {
        return new StyleBoxFlat
        {
            BgColor = color,
            BorderColor = color.Lightened(0.25f),
            BorderWidthLeft = 1,
            BorderWidthTop = 1,
            BorderWidthRight = 1,
            BorderWidthBottom = 1,
            CornerRadiusTopLeft = 3,
            CornerRadiusTopRight = 3,
            CornerRadiusBottomRight = 3,
            CornerRadiusBottomLeft = 3,
        };
    }
}

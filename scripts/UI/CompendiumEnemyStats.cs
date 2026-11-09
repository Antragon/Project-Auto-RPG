namespace Game.scripts.UI;

using Godot;
using Units;
using static CompendiumUIHelpers;

public partial class CompendiumEnemyStats : GridContainer
{
    public void ShowEnemy(UnitData? unitData, int count)
    {
        ClearChildren(this);
        if (unitData is null)
        {
            AddChild(CreateLabel("Base stats unavailable"));
            return;
        }

        AddStat("Max HP", new Unit(unitData).MaxHp, count >= 20);
        AddStat("Vitality", unitData.Vitality, count >= 20);
        AddStat("Strength", unitData.Strength, count >= 20);
        AddStat("Dexterity", unitData.Dexterity, count >= 20);
        AddStat("Magic", unitData.Magic, count >= 20);
        AddStat("Speed", unitData.Speed, count >= 20);
    }

    private void AddStat(string name, int value, bool revealed)
    {
        AddChild(CreateLabel(name));
        AddChild(CreateLabel(revealed ? value.ToString() : "?"));
    }
}

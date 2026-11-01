namespace Game.scripts.Units;

using System.Collections.Generic;
using System.Linq;

public sealed class StatModifier : Dictionary<object, int>
{
    public int Value => Values.Sum();
}

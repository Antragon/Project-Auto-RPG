namespace Game.scripts.Units;

using System.Collections.Generic;
using System.Linq;

public sealed class DamageModifier : Dictionary<object, int>
{
    public int Value => Values.Sum();
}

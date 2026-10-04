namespace Game.scripts;

using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Units;
using DungeonNode = Dungeon.Dungeon;

public partial class GameRoot : SceneRootBase
{
    private DungeonNode Dungeon => field ??= GetNode<DungeonNode>("Dungeon");

    public override void _Ready()
    {
        Dungeon.Enter("forest");
    }

    protected override void AddLocalDependencies(ServiceCollection services)
    {
        services.AddSingleton<SaveRepository>();
        services.AddSingleton<CharacterRosterFactory>();
        services.AddSingleton<CharacterRoster>(serviceProvider =>
            serviceProvider.GetRequiredService<CharacterRosterFactory>().Create());
        services.AddSingleton(_ => GetDescendant<CharacterFormation>());
    }
}

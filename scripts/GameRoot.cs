namespace Game.scripts;

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Skills;
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
        services.AddSingleton<SkillAssignments>();
        services.AddSingleton<SkillRepository>();
        services.AddSingleton<CharacterRepositoryFactory>();
        services.AddSingleton<CharacterRepository>(serviceProvider =>
            serviceProvider.GetRequiredService<CharacterRepositoryFactory>().Create());
        services.AddSingleton(_ => GetDescendant<CharacterFormation>());
    }
}

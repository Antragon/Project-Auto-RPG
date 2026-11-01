namespace Game.scripts;

using Equipment;
using Loot;
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
        services.AddSingleton<SaveDataWriter>();
        services.AddSingleton<EnemyDefeatRepository>();
        services.AddSingleton<LootRepository>();
        services.AddSingleton<EquipmentAssignments>();
        services.AddSingleton<EquipmentRepositoryFactory>();
        services.AddSingleton<EquipmentRepository>(serviceProvider =>
            serviceProvider.GetRequiredService<EquipmentRepositoryFactory>().Create());
        services.AddSingleton<SkillAssignments>();
        services.AddSingleton<SkillRepositoryFactory>();
        services.AddSingleton<SkillRepository>(serviceProvider =>
            serviceProvider.GetRequiredService<SkillRepositoryFactory>().Create());
        services.AddSingleton<CharacterRepositoryFactory>();
        services.AddSingleton<CharacterRepository>(serviceProvider =>
            serviceProvider.GetRequiredService<CharacterRepositoryFactory>().Create());
        services.AddSingleton(_ => GetDescendant<CharacterFormation>());
    }
}

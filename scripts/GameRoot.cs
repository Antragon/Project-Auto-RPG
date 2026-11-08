namespace Game.scripts;

using Equipment;
using Extensions;
using Loot;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Skills;
using Units;
using DungeonNode = Dungeon.Dungeon;

public partial class GameRoot : SceneRootBase
{
    private DungeonNode Dungeon => field ??= GetNode<DungeonNode>("Dungeon");

    private SaveRepository SaveRepository => field ??= this.Root.Get<SaveRepository>();

    public override void _Ready()
    {
        var dungeon = SaveRepository.SaveData.PlayerState.SelectedDungeon;
        Dungeon.Enter(string.IsNullOrWhiteSpace(dungeon) ? SavedPlayerState.DefaultDungeon : dungeon);
        if (Dungeon.Data is null) Dungeon.Enter(SavedPlayerState.DefaultDungeon);
    }

    protected override void AddLocalDependencies(ServiceCollection services)
    {
        services.AddSingleton<SaveRepository>();
        services.AddSingleton<SaveDataWriter>();
        services.AddSingleton<EnemyDefeatRepository>();
        services.AddSingleton<EnemyScalingService>();
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

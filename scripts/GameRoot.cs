namespace Game.scripts;

using System.Linq;
using Dungeon;
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

    private SaveDataWriter SaveDataWriter => field ??= this.Root.Get<SaveDataWriter>();

    private CharacterRepository CharacterRepository => field ??= this.Root.Get<CharacterRepository>();

    private CharacterFormation CharacterFormation => field ??= this.Root.Get<CharacterFormation>();

    private bool _savePending;

    public override void _Ready()
    {
        foreach (var slot in CharacterFormation.GetChildrenOfType<CharacterSlot>())
        {
            if (SaveRepository.SaveData.PlayerState.Team.TryGetValue(slot.SlotIndex, out var characterId)
                && CharacterRepository.TryGetCharacter(characterId, out var character))
            {
                slot.Assign(character);
            }
        }

        CharacterFormation.Changed += OnFormationChanged;
        foreach (var character in CharacterRepository.Characters.Values)
        {
            character.Unit.PropertyChanged += OnCharacterUnitPropertyChanged;
        }

        var dungeon = SaveRepository.SaveData.PlayerState.SelectedDungeon;
        Dungeon.Enter(string.IsNullOrWhiteSpace(dungeon) ? SavedPlayerState.DefaultDungeon : dungeon);
        if (Dungeon.Data is null) Dungeon.Enter(SavedPlayerState.DefaultDungeon);
    }

    public override void _Process(double delta)
    {
        if (_savePending)
        {
            _savePending = false;
            SaveDataWriter.Save();
        }
    }

    public override void _ExitTree()
    {
        CharacterFormation.Changed -= OnFormationChanged;
        foreach (var character in CharacterRepository.Characters.Values)
        {
            character.Unit.PropertyChanged -= OnCharacterUnitPropertyChanged;
        }

        if (_savePending) SaveDataWriter.Save();
        base._ExitTree();
    }

    private void OnFormationChanged()
    {
        SaveRepository.SaveData.PlayerState.Team = CharacterFormation.GetChildrenOfType<CharacterSlot>()
            .Where(slot => slot.Character is not null)
            .ToDictionary(slot => slot.SlotIndex, slot => slot.Character!.Id);
        _savePending = true;
    }

    private void OnCharacterUnitPropertyChanged(Unit unit, string propertyName)
    {
        if (propertyName == nameof(Unit.Hp)) _savePending = true;
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

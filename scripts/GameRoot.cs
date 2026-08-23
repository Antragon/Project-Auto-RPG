namespace Game.scripts;

using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Units;

public partial class GameRoot : SceneRootBase
{
    protected override void AddLocalDependencies(ServiceCollection services)
    {
        services.AddSingleton<SaveRepository>();
        services.AddSingleton<CharacterFormationService>();
    }
}

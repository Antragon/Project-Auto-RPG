namespace Game.scripts;

using Microsoft.Extensions.DependencyInjection;
using Persistence;

public partial class GameRoot : SceneRootBase
{
    protected override void AddLocalDependencies(ServiceCollection services)
    {
        services.AddSingleton<SaveRepository>();
    }
}
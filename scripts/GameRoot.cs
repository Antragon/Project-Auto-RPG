namespace Game.scripts;

using Microsoft.Extensions.DependencyInjection;

public partial class GameRoot : SceneRootBase {
    protected override void AddLocalDependencies(ServiceCollection services) {
        // _services.AddSingleton(GetDescendant<...>(this));
    }
}
namespace Game.scripts;

public interface ISceneRoot {
    T Get<T>(object? serviceKey = null)
        where T : notnull;
}
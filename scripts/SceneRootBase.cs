namespace Game.scripts;

using System;
using System.Collections.Generic;
using Godot;
using Microsoft.Extensions.DependencyInjection;

public abstract partial class SceneRootBase : Node, ISceneRoot
{
    private readonly ServiceCollection _services = [];

    private ServiceProvider ServiceProvider => field ??= _services.BuildServiceProvider();

    public override void _EnterTree()
    {
        AddLocalDependencies(_services);
    }

    protected abstract void AddLocalDependencies(ServiceCollection services);

    public T Get<T>(object? serviceKey = null)
        where T : notnull
    {
        return ServiceProvider.GetRequiredKeyedService<T>(serviceKey);
    }

    protected static T GetDescendant<T>(Node node)
    {
        var queue = new Queue<Node>();
        foreach (var child in node.GetChildren())
        {
            queue.Enqueue(child);
        }

        while (true)
        {
            if (queue.Count == 0)
            {
                throw new Exception("Could not find descendant of type " + typeof(T));
            }

            var current = queue.Dequeue();

            if (current is T target)
            {
                return target;
            }

            foreach (var child in current.GetChildren())
            {
                queue.Enqueue(child);
            }
        }
    }
}

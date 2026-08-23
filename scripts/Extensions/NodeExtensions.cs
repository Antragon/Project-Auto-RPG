namespace Game.scripts.Extensions;

using System;
using System.Linq;
using Godot;

public static class NodeExtensions
{
    extension(Node node)
    {
        public ISceneRoot Root => (ISceneRoot)node.GetTree().CurrentScene;

        public T GetAncestor<T>()
        {
            while (true)
            {
                var parentNode = node.GetParent();
                if (parentNode is T parent)
                {
                    return parent;
                }

                node = parentNode;
            }
        }

        public T GetSibling<T>()
        {
            var parent = node.GetParent();
            var sibling = parent.GetChildOfType<T>();
            return sibling;
        }

        public T GetChildOfType<T>()
        {
            return node.GetChildren().OfType<T>().First();
        }

        public void ExecuteSafely(Action action)
        {
            if (node.IsInsideTree())
            {
                action();
            }
        }
    }
}

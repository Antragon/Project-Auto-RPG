namespace Game.scripts.Extensions;

using System.Collections.Generic;
using Godot;

public static class DirAccessExtensions
{
    extension(DirAccess)
    {
        public static IEnumerable<string> GetResourceFilesRecursivelyAt(string basePath)
        {
            return DirAccess.GetFilesRecursivelyAt(basePath, ".tres");
        }

        public static IEnumerable<string> GetFilesRecursivelyAt(string basePath, string fileEnding)
        {
            var queue = new Queue<string>();
            queue.Enqueue(basePath);

            while (queue.Count > 0)
            {
                var path = queue.Dequeue();
                foreach (var fileName in DirAccess.GetFilesAt(path))
                {
                    if (fileName.EndsWith(fileEnding))
                    {
                        yield return path.PathJoin(fileName);
                    }
                }

                foreach (var directory in DirAccess.GetDirectoriesAt(path))
                {
                    queue.Enqueue(path.PathJoin(directory));
                }
            }
        }
    }
}

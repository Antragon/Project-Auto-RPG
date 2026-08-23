namespace Game.scripts.Extensions;

using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Godot;
using Godot.Collections;

public static class ResourceExtensions
{
    extension(Resource resource)
    {
        public string Name => Path.GetFileNameWithoutExtension(resource.ResourcePath);
    }

    extension<[MustBeVariant] T>(Array<T> array)
        where T : Resource
    {
        public ReadOnlyDictionary<string, T> ToReadOnlyDictionary()
        {
            var dictionary = array.ToDictionary(x => x.Name, x => x);
            var readOnlyDictionary = new ReadOnlyDictionary<string, T>(dictionary);
            return readOnlyDictionary;
        }
    }
}

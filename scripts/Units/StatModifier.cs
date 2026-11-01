namespace Game.scripts.Units;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public sealed class StatModifier : IDictionary<object, int>, IReadOnlyDictionary<object, int>
{
    private readonly Dictionary<object, int> _modifiers = [];

    public event Action? Changed;

    public int Value => _modifiers.Values.Sum();

    public int this[object key]
    {
        get => _modifiers[key];
        set
        {
            if (_modifiers.TryGetValue(key, out var previous) && previous == value) return;
            _modifiers[key] = value;
            Changed?.Invoke();
        }
    }

    public ICollection<object> Keys => _modifiers.Keys;

    public ICollection<int> Values => _modifiers.Values;

    IEnumerable<object> IReadOnlyDictionary<object, int>.Keys => Keys;

    IEnumerable<int> IReadOnlyDictionary<object, int>.Values => Values;

    public int Count => _modifiers.Count;

    public bool IsReadOnly => false;

    public void Add(object key, int value)
    {
        _modifiers.Add(key, value);
        Changed?.Invoke();
    }

    public void Add(KeyValuePair<object, int> item) => Add(item.Key, item.Value);

    public bool Remove(object key)
    {
        if (!_modifiers.Remove(key)) return false;
        Changed?.Invoke();
        return true;
    }

    public bool Remove(KeyValuePair<object, int> item)
    {
        return Contains(item) && Remove(item.Key);
    }

    public void Clear()
    {
        if (_modifiers.Count == 0) return;
        _modifiers.Clear();
        Changed?.Invoke();
    }

    public bool ContainsKey(object key) => _modifiers.ContainsKey(key);

    public bool TryGetValue(object key, out int value) => _modifiers.TryGetValue(key, out value);

    public bool Contains(KeyValuePair<object, int> item)
    {
        return ((ICollection<KeyValuePair<object, int>>)_modifiers).Contains(item);
    }

    public void CopyTo(KeyValuePair<object, int>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<object, int>>)_modifiers).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<object, int>> GetEnumerator() => _modifiers.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

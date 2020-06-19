using System;
using UnityEngine;

namespace Gameframe.Serializable
{
  /// <summary>
  /// Interface for a key value pair.
  /// </summary>
  /// <typeparam name="TKey">Key type.</typeparam>
  /// <typeparam name="TValue">Value type.</typeparam>
  public interface IKeyValuePair<TKey, TValue>
  {
    TKey Key { get; set; }
    TValue Value { get; set; }
  }

  /// <summary>
  /// Serializable KeyValuePair
  /// </summary>
  /// <typeparam name="TKey">Key type. Must be serializable for serialization to function.</typeparam>
  /// <typeparam name="TValue">Value type. Must be serializabe for serialization to function.</typeparam>
  [Serializable]
  public class SerializablePair<TKey,TValue> : IKeyValuePair<TKey,TValue>
  {
    [SerializeField]
    private TKey _key;
    [SerializeField]
    private TValue _value;

    public SerializablePair()
    {
    }
        
    public SerializablePair(TKey key, TValue value)
    {
      _key = key;
      _value = value;
    }
            
    public TKey Key
    {
      get => _key;
      set => _key = value;
    }

    public TValue Value
    {
      get => _value;
      set => _value = value;
    }
  }
}
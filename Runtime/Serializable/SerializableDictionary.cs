using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.Serializable
{
    /// <summary>
    /// Used internally by the SerializableDictionary.
    /// This interface should be implemented on a serializable class
    /// It can then be used with SerializableDictionary to serialize a dictionary as a list
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IKeyValuePair<TKey, TValue>
    {
        TKey Key { get; set; }
        TValue Value { get; set; }
    }

    /// <summary>
    /// SerializableDictionary is an abstract dictionary class that uses
    /// a private abstract list property to implement its serializable state
    /// while at the same time keeping an internal dictionary to implement dictionary operations
    /// </summary>
    /// <typeparam name="TKey">Key</typeparam>
    /// <typeparam name="TValue">Value</typeparam>
    /// <typeparam name="TKeyValuePair">Should be a IKeyValuePair type that is also serializable</typeparam>
    public abstract class SerializableDictionary<TKey,TValue,TKeyValuePair> : IDictionary<TKey, TValue> where TKeyValuePair : IKeyValuePair<TKey,TValue>
    {
        /// <summary>
        /// This property should return the serialized List<TKeyValuePair> field in the child class
        /// </summary>
        protected abstract List<TKeyValuePair> KeyValueList { get; }
        
        /// <summary>
        /// Returns a new KeyValue pair that can be inserted into the internal list
        /// </summary>
        /// <returns>A new key value pair</returns>
        protected abstract TKeyValuePair NewPair();

        private readonly Dictionary<TKey, TValue> _internalDict = new Dictionary<TKey, TValue>();
        
        private bool _internalDictPopulated = false;

        public TValue this[TKey index]
        {
            get => _internalDict[index];
            set => AddOrSetValue(index, value);
        }

        public int Count => KeyValueList.Count;

        public bool IsReadOnly => false;

        public ICollection<TKey> Keys
        {
            get
            {
                BuildDict();
                return _internalDict.Keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                BuildDict();
                return _internalDict.Values;
            }
        }

        public void Clear()
        {
            _internalDict.Clear();
            KeyValueList.Clear();
        }

        public void Add(TKey key, TValue value)
        {
            BuildDict();

            _internalDict.Add(key, value);
            var pair = NewPair();
            pair.Key = key;
            pair.Value = value;
            KeyValueList.Add(pair);
        }

        public void Add(KeyValuePair<TKey,TValue> pair)
        {
            Add(pair.Key, pair.Value);
        }

        public bool Contains(KeyValuePair<TKey, TValue> pair)
        {
            BuildDict();
            return ((ICollection<KeyValuePair<TKey, TValue>>)_internalDict).Contains(pair);
        }

        public bool Remove(KeyValuePair<TKey, TValue> pair)
        {
            return _internalDict.Remove(pair.Key);
        }

        public void CopyTo(KeyValuePair<TKey,TValue>[] array, int index)
        {
            BuildDict();
            ((ICollection<KeyValuePair<TKey, TValue>>)_internalDict).CopyTo(array, index);
        }

        public bool ContainsKey(TKey key)
        {
            BuildDict();
            return _internalDict.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            BuildDict();

            if ( _internalDict.Remove(key) )
            {
                //Remove from list
                for ( int i = 0; i < KeyValueList.Count; i++ )
                {
                    var pair = KeyValueList[i];
                    if ( pair.Key.Equals(key) )
                    {
                        KeyValueList.RemoveAt(i);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            BuildDict();
            return _internalDict.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            BuildDict();
            return _internalDict.GetEnumerator();// as IEnumerator<KeyValuePair<T,U>>;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            BuildDict();
            return _internalDict.GetEnumerator();
        }

        private void BuildDict()
        {
            if ( _internalDict.Count != KeyValueList.Count || !_internalDictPopulated )
            {
                _internalDict.Clear();
                for ( int i = 0; i < KeyValueList.Count; i++ )
                {
                    var pair = KeyValueList[i];
                    _internalDict.Add(pair.Key, pair.Value);
                }
            }
            _internalDictPopulated = true;
        }

        private void AddOrSetValue(TKey key, TValue value)
        {
            BuildDict();

            if ( _internalDict.ContainsKey(key) )
            {
                //Update existing value
                _internalDict[key] = value;
                GetPair(key).Value = value;
            }
            else
            {
                //Add new value
                Add(key, value);
            }
        }

        private IKeyValuePair<TKey,TValue> GetPair(TKey key)
        {
            for ( int i = 0; i < KeyValueList.Count; i++ )
            {
                var pair = KeyValueList[i];
                if ( pair.Key.Equals(key) )
                {
                    return pair;
                }
            }
            return null;
        }

    }
    
    public abstract class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        public virtual TValue this[TKey key]
        {
            get => _dictionary[key];
            set
            {
                _dictionary[key] = value;
                Set(key, value);
            }
        }

        public TValue GetOrDefault(TKey key, TValue defaultValue)
        {
            return TryGetValue(key, out var outValue) ? outValue : defaultValue;
        }

        public bool TryGetValue(TKey key, out TValue outValue)
        {
            return _dictionary.TryGetValue(key, out outValue);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public virtual void OnAfterDeserialize()
        {
            _dictionary = new Dictionary<TKey, TValue>();
            int count = InternalCount();
            for (int i = 0; i < count; i++)
            {
                _dictionary[GetKey(i)] = GetValue(i);
            }
        }

        public virtual void OnBeforeSerialize()
        {
        }

        public int Count => _dictionary.Count;

        protected abstract int InternalCount();
        protected abstract void Set(TKey key, TValue value);
        protected abstract TKey GetKey(int i);
        protected abstract TValue GetValue(int i);
    }
    

}



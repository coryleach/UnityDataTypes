using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.Serializable
{
    /// <summary>
    /// Abstract class that maintains maintains an internal dictionary which is kept in sync with an underlying serializable state
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public abstract class BaseSerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        /// <summary>
        /// Tries to get a value out of the dictionary
        /// If that value is not found the default value will be returned without adding it to the dictionary
        /// </summary>
        /// <param name="key">Key to get</param>
        /// <param name="defaultValue">Default value to return if Key is not found</param>
        /// <returns>Value for the key</returns>
        public TValue GetOrDefault(TKey key, TValue defaultValue)
        {
            return TryGetValue(key, out var outValue) ? outValue : defaultValue;
        }

        #region IDictionary<TKey,TValue>

        public virtual TValue this[TKey key]
        {
            get => _dictionary[key];
            set
            {
                _dictionary[key] = value;
                Set(key, value);
            }
        }

        public ICollection<TKey> Keys => _dictionary.Keys;

        public ICollection<TValue> Values => _dictionary.Values;

        public void Add(TKey key, TValue value)
        {
            Set(key, value);
            _dictionary.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (_dictionary.Remove(key))
            {
                RemoveKey(key);
                return true;
            }

            return false;
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

        #endregion

        #region ICollection<KeyValuePair<TKey, TValue>>

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Set(item.Key, item.Value);
            _dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            InternalClear();
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>) _dictionary).Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>) _dictionary).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            RemoveKey(item.Key);
            return _dictionary.Remove(item.Key);
        }

        public int Count => InternalCount();

        public virtual bool IsReadOnly { get; } = false;

        #endregion

        #region ISerializationCallbackReciever

        public virtual void OnAfterDeserialize()
        {
            //Build the dictionary from the internal state
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

        #endregion

        #region Abstract Properties and Methods

        /// <summary>
        /// This method should return the size of the backing serializable list
        /// </summary>
        /// <returns>size of the list</returns>
        protected abstract int InternalCount();

        /// <summary>
        /// Sets a key value pair.
        /// Should add automatically if the key does not exist.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        protected abstract void Set(TKey key, TValue value);

        /// <summary>
        /// Gets the key for a given index
        /// </summary>
        /// <param name="i">index</param>
        /// <returns>Key at the given index</returns>
        protected abstract TKey GetKey(int i);

        /// <summary>
        /// Get the value for a given index
        /// </summary>
        /// <param name="i">index</param>
        /// <returns>Value for the given index</returns>
        protected abstract TValue GetValue(int i);

        /// <summary>
        /// Remove the key value pair for the given key
        /// </summary>
        /// <param name="key">key to be removed</param>
        protected abstract void RemoveKey(TKey key);

        /// <summary>
        /// This method gets called if the dictionary is cleared
        /// This method should clear the backing serializable list
        /// </summary>
        protected abstract void InternalClear();

        #endregion

    }

}




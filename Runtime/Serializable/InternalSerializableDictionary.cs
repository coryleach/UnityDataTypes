using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameframe.Serializable
{
    /// <summary>
    /// This class maintains maintains an internal dictionary that is kept in sync with a serialized state
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public abstract class InternalSerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
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
            Set(key,value);
            _dictionary.Add(key,value);
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
            Set(item.Key,item.Value);
            _dictionary.Add(item.Key,item.Value);
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
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
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

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IKeyValuePair<TKey, TValue>
    {
        TKey Key { get; set; }
        TValue Value { get; set; }
    }

    /// <summary>
    /// Serializable Dictionary backed by an internal list of serializable key pair values
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public abstract class SerializableDictionary<TKey, TValue> : InternalSerializableDictionary<TKey, TValue>
    {
        protected abstract ISerializablePairList<TKey,TValue> PairList { get; }
        
        protected override int InternalCount()
        {
            return PairList.Count;
        }

        protected override void Set(TKey key, TValue value)
        {
            var pair = PairList.FirstOrDefault(x => x.Key.Equals(key));
            if (pair != null)
            {
                pair.Value = value;
            }
            else
            {
                AddPair(key,value);
            }
        }

        private void AddPair(TKey key, TValue value)
        {
            PairList.Add(key,value);
        }
        
        protected override TKey GetKey(int i)
        {
            return PairList[i].Key;
        }

        protected override TValue GetValue(int i)
        {
            return PairList[i].Value;
        }

        protected override void RemoveKey(TKey key)
        {
            for (var i = 0; i < PairList.Count; i++)
            {
                if (!PairList[i].Key.Equals(key))
                {
                    continue;
                }
                
                PairList.RemoveAt(i);
                return;
            }
        }

        protected override void InternalClear()
        {
            PairList.Clear();
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
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

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface ISerializablePairList<TKey, TValue> : IList<IKeyValuePair<TKey, TValue>>
    {
        void Add(TKey key, TValue value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPair"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class SerializablePairList<TPair,TKey,TValue> : ISerializablePairList<TKey,TValue> where TPair : SerializablePair<TKey,TValue>, new()
    {

        [SerializeField]
        private List<TPair> list = new List<TPair>();

        public void Add(TKey key, TValue value)
        {
            list.Add(new TPair{Key = key, Value = value});
        }
        
        IEnumerator<IKeyValuePair<TKey, TValue>> IEnumerable<IKeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return list.Cast<IKeyValuePair<TKey,TValue>>().GetEnumerator();
        }

        public IEnumerator<TPair> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) list).GetEnumerator();
        }

        public void Add(TPair item)
        {
            list.Add(item);
        }

        public void Add(IKeyValuePair<TKey, TValue> item)
        {
            list.Add(item == null ? null : new TPair {Key = item.Key, Value = item.Value});
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(IKeyValuePair<TKey, TValue> item)
        {
            return list.Contains(item);
        }

        public void CopyTo(IKeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (array.Length - arrayIndex < list.Count)
            {
                throw new ArgumentException("Not enough space in array to perform copy with the given parameters");
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }
            
            for (var i = 0; i < list.Count; i++)
            {
                array[i + arrayIndex] = list[i];
            }
        }

        public bool Remove(IKeyValuePair<TKey, TValue> item)
        {
            return list.Remove(item as TPair);
        }

        public bool Contains(TPair item)
        {
            return list.Contains(item);
        }

        public void CopyTo(TPair[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public bool Remove(TPair item)
        {
            return list.Remove(item);
        }

        public int Count => list.Count;

        public bool IsReadOnly => false;

        public int IndexOf(TPair item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, TPair item)
        {
            list.Insert(index, item);
        }

        public int IndexOf(IKeyValuePair<TKey, TValue> item)
        {
            switch (item)
            {
                case null:
                    return list.IndexOf(null);
                case TPair pair:
                    return list.IndexOf(pair);
                default:
                    return list.IndexOf(new TPair{Key=item.Key,Value=item.Value});
            }
        }

        public void Insert(int index, IKeyValuePair<TKey, TValue> item)
        {
            switch (item)
            {
                case null:
                    list.Insert(index,null);
                    break;
                case TPair pair:
                    list.Insert(index,pair);
                    break;
                default:
                    list.Insert(index, new TPair{Key=item.Key,Value=item.Value});
                    break;
            }
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        IKeyValuePair<TKey, TValue> IList<IKeyValuePair<TKey, TValue>>.this[int index]
        {
            get => list[index];
            set
            {
                switch (value)
                {
                    case null:
                        break;
                    case TPair pair:
                        list[index] = pair;
                        break;
                    default:
                        list[index] = new TPair{ Key = value.Key, Value = value.Value};
                        break;
                }
            }
        }

        public TPair this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }
    }
    
}




using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameframe.Serializable
{
    /// <summary>
    /// Interface for a list of IKeyValuePair items
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public interface ISerializablePairList<TKey, TValue> : IList<IKeyValuePair<TKey, TValue>>
    {
        void Add(TKey key, TValue value);
    }

    /// <summary>
    /// Serializable list of Key-Value pairs
    /// </summary>
    /// <typeparam name="TPair">Type that implements SerializablePair<TKey,TValue></typeparam>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Pair type</typeparam>
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
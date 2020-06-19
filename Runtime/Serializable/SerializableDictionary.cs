using System.Linq;

namespace Gameframe.Serializable
{
    /// <summary>
    /// SerializableDictionary implemented using an ISerializablePairList
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public abstract class SerializableDictionary<TKey, TValue> : BaseSerializableDictionary<TKey, TValue>
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
}

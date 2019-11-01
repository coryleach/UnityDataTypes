using System;
using System.Collections;
using System.Collections.Generic;

namespace Gameframe.DataTypes
{

  public class ListDictionary<TKey, TValue> : IDictionary<TKey,List<TValue>>
  {

    private Dictionary<TKey, List<TValue>> dictionary = new Dictionary<TKey, List<TValue>>();

    public ListDictionary()
    {

    }

    public void Add(TKey key, TValue value)
    {
      List<TValue> list = null;

      //Try and get existing list for this key
      if ( !dictionary.TryGetValue(key, out list) )
      {
        //No list found so create and add it
        list = new List<TValue>();
        dictionary.Add(key, list);
      }

      //Add the value to the list
      list.Add(value);
    }

    public bool Remove(TKey key, TValue value)
    {
      List<TValue> list = null;

      //Try to get the list out of the dictionary and return false if it's not found
      if (!dictionary.TryGetValue(key, out list))
      {
        return false;
      }

      //Return false if we fail to remove item
      if ( !list.Remove(value) )
      {
        return false;
      }

      //Remove the list from the dictionary if it's empty
      if ( list.Count == 0 )
      {
        dictionary.Remove(key);
      }

      return true;
    }

#region IDictionary Interface

    public List<TValue> this[TKey key]
    {
      get
      {
        return dictionary[key];
      }
      set
      {
        dictionary[key] = value;
      }
    }

    public ICollection<TKey> Keys { get { return dictionary.Keys; } }

    public ICollection<List<TValue>> Values { get { return dictionary.Values; } }

    public int Count { get { return dictionary.Count; } }

    public bool IsReadOnly { get { return false; } }

    public void Add(TKey key, List<TValue> value)
    {
      dictionary.Add(key, value);
    }

    public void Add(KeyValuePair<TKey, List<TValue>> item)
    {
      dictionary.Add(item.Key,item.Value);
    }

    public void Clear()
    {
      dictionary.Clear();
    }

    public bool Contains(KeyValuePair<TKey, List<TValue>> item)
    {
      throw new NotImplementedException();
    }

    public bool ContainsKey(TKey key)
    {
      return dictionary.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<TKey, List<TValue>>[] array, int arrayIndex)
    {
      ((IDictionary<TKey, List<TValue>>)dictionary).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator()
    {
      return ((IDictionary<TKey, List<TValue>>)dictionary).GetEnumerator();
    }

    public bool Remove(TKey key)
    {
      return dictionary.Remove(key);
    }

    public bool Remove(KeyValuePair<TKey, List<TValue>> item)
    {
      throw new NotImplementedException();
    }

    public bool TryGetValue(TKey key, out List<TValue> value)
    {
      return dictionary.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return dictionary.GetEnumerator();
    }

#endregion

  }

}
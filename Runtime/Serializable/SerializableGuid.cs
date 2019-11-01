using System.Collections.Generic;
using System;
using Random = System.Random;

namespace Gameframe.DataTypes.Serializable
{
  [Serializable]
  public class SerializableGuid : UnityEngine.ISerializationCallbackReceiver
  {
    private static Random _rng = null;
    private static Random RandomGenerator => _rng ?? (_rng = new Random());
    private static readonly HashSet<int> Used = new HashSet<int>();
    private static readonly SerializableGuid _invalid = new SerializableGuid(0);

    public static SerializableGuid NewGuid()
    {
      return new SerializableGuid();
    }

    public static SerializableGuid Invalid()
    {
      return _invalid;
    }

    public static bool IsUsed(int id)
    {
      return Used.Contains(id);
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        if ( _id != 0 )
        {
          Used.Add(_id);
        }
    }

    private SerializableGuid(int id)
    {
      _id = id;
    }

    private SerializableGuid()
    {
        int count = 0;
        while (_id == 0 || Used.Contains(_id) )
        {
          if ( count > 100 )
          {
            throw new Exception("GameGuid failed to generate a unique id in less than 100 steps. Increase the size of the id space.");
          }
          _id = RandomGenerator.Next();
          count++;
        }
        Used.Add(_id);
    }

    ~SerializableGuid()
    {
        Used.Remove(_id);
    }

    private readonly int _id = 0;
    public int Id => _id;

    public bool IsValid()
    {
      return _id != 0;
    }

    public override bool Equals(object obj)
    {
      if ( obj is SerializableGuid other )
      {
        return other._id == _id;
      }
      return base.Equals(obj);
    }

    public override string ToString()
    {
      return base.ToString();
    }

    public override int GetHashCode()
    {
      return _id;
    }

  }
}
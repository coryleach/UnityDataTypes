using System;
using UnityEngine;

namespace Gameframe.DataTypes.Serializable
{

  [Serializable]
  public class Bitmask
  {

    [SerializeField]
    UInt32 bitmask = 0;

    public UInt32 RawValue
    {
      get => bitmask;
      set => bitmask = value;
    }

    public void Set(UInt32 flag, bool value)
    {
      if (value)
      {
        bitmask = (bitmask | flag);
      }
      else
      {
        bitmask = (bitmask & ~flag);
      }
    }

    public bool Get(UInt32 flag)
    {
      return (bitmask & flag) != 0;
    }

  }

}

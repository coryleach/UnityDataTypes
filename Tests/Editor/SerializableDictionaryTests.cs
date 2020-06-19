using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameframe.Serializable;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Gameframe.DataTypes.Tests.Editor
{
  public class SerialziableDictionaryTests
  {
    [Serializable]
    public class TestListDict : SerializableDictionary<int,int>
    {
      [Serializable] public class MyPair : SerializablePair<int, int> { }
      
      [Serializable] public class MyList : SerializablePairList<MyPair,int,int> {}
      
      [SerializeField] 
      private MyList list = new MyList();
      
      protected override ISerializablePairList<int,int> PairList => list;
    }

    [Serializable]
    public class MyTest : MyListDict<int, int>
    {
      
    }
    
    [Serializable]
    public class MyListDict<TKey, TValue> : SerializableDictionary<TKey,TValue>
    {
      [Serializable] public class MyPair : SerializablePair<TKey, TValue> { }
      
      [Serializable] public class MyList : SerializablePairList<MyPair,TKey,TValue> {}
      
      [SerializeField] 
      private MyList list = new MyList();
      
      protected override ISerializablePairList<TKey,TValue> PairList => list;
    }


    [Serializable]
    public class TestPair : SerializablePair<int, int>
    {
      public TestPair()
      {
      }
      public TestPair(int key, int value) : base(key, value)
      {
      }
    }

    [Serializable]
    public class TestPairList : SerializablePairList<TestPair,int,int>
    {
    }

    [Test]
    public void SerializePairType()
    {
      var myPair = new TestPair();
      myPair.Key = 1;
      myPair.Value = 10;
      var json = JsonUtility.ToJson(myPair);
      var deserialized = JsonUtility.FromJson<TestPair>(json);
      Debug.Log(json);
      Assert.IsTrue(deserialized != null);
      Assert.IsTrue(deserialized.Key == 1);
      Assert.IsTrue(deserialized.Value == 10);
    }
    
    [Test]
    public void SerializePairListType()
    {
      var myPairList = new TestPairList();
      myPairList.Add(1,10);
      var json = JsonUtility.ToJson(myPairList);
      Debug.Log(json);
      var deserialized = JsonUtility.FromJson<TestPairList>(json);
      Assert.IsTrue(deserialized != null);
      Assert.IsTrue(deserialized.Count == 1);
      Assert.IsTrue(deserialized[0].Key == 1);
      Assert.IsTrue(deserialized[0].Value == 10);
    }
    
    [Test]
    public void CanCreate()
    {
      var listDict = new TestListDict();
      Assert.IsTrue(listDict != null);
    }
    
    [Test]
    public void Add()
    {
      var listDict = new TestListDict();
      
      listDict.Add(1,10);
      
      Assert.IsTrue(listDict.ContainsKey(1));
      Assert.IsTrue(listDict.Count == 1);
      Assert.IsTrue(listDict.TryGetValue(1, out var value));
      Assert.IsTrue(value == 10);
    }

    [Test]
    public void CanSerializeAndDeserialize()
    {
      var listDict = new MyTest();
      listDict.Add(1,10);

      var json = JsonUtility.ToJson(listDict);
      Debug.Log(json);
      Assert.IsFalse(string.IsNullOrEmpty(json));

      var deserializedDict = JsonUtility.FromJson<MyTest>(json);
      Assert.IsTrue(deserializedDict.ContainsKey(1));
      Assert.IsTrue(deserializedDict.Count == 1);
      Assert.IsTrue(deserializedDict.TryGetValue(1, out var value));
      Assert.IsTrue(value == 10);
    }
  }
}
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class MergebaleObjectsLineETest
    {
        [Test]
        public void CheckMaxLevel()
        {
            GameObject _testPlacable = GameObject.Instantiate(new GameObject());
            Mergeable _placable = _testPlacable.AddComponent<Mergeable>();
            GameObject _testPlacable1 = GameObject.Instantiate(new GameObject());
            Mergeable _placable1 = _testPlacable1.AddComponent<Mergeable>();
            GameObject _testPlacable2 = GameObject.Instantiate(new GameObject());
            Mergeable _placable2 = _testPlacable2.AddComponent<Mergeable>();

            MergebaleObjectsLine _mergeLine = new MergebaleObjectsLine();

            Mergeable[] _testMergeLine = new Mergeable[3];
            _testMergeLine[0] = _placable;
            _testMergeLine[1] = _placable1;
            _testMergeLine[2] = _placable2;

            // Проблема рефлексии в том, что если поменять имя поля, то придется переписывать
            Type _type = _mergeLine.GetType();
            FieldInfo _lineProperty = _type.GetField("_objectLevels", BindingFlags.NonPublic | BindingFlags.Instance);
            _lineProperty.SetValue(_mergeLine, _testMergeLine);

            Assert.AreEqual(2, _mergeLine.MaxLevel);
        }

        [Test]
        public void TestCurrentLevel()
        {
            GameObject _testPlacable = GameObject.Instantiate(new GameObject());
            Mergeable _placable = _testPlacable.AddComponent<Mergeable>();
            GameObject _testPlacable1 = GameObject.Instantiate(new GameObject());
            Mergeable _placable1 = _testPlacable1.AddComponent<Mergeable>();
            GameObject _testPlacable2 = GameObject.Instantiate(new GameObject());
            Mergeable _placable2 = _testPlacable2.AddComponent<Mergeable>();

            MergebaleObjectsLine _mergeLine = ScriptableObject.CreateInstance<MergebaleObjectsLine>();

            Mergeable[] _testMergeLine = new Mergeable[3];
            _testMergeLine[0] = _placable;
            _testMergeLine[1] = _placable1;
            _testMergeLine[2] = _placable2;

            // Проблема рефлексии в том, что если поменять имя поля, то придется переписывать
            Type _type = _mergeLine.GetType();
            FieldInfo _lineProperty = _type.GetField("_objectLevels", BindingFlags.NonPublic | BindingFlags.Instance);
            _lineProperty.SetValue(_mergeLine, _testMergeLine);

            Assert.AreEqual(_placable2, _mergeLine.GetCurrentLevelObject(2));
        }
    }
}

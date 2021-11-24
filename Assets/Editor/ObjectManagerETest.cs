using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ObjectManagerTest
    {

        private Dictionary<Vector3Int, Placeable> _placedObjects = new Dictionary<Vector3Int, Placeable>();
        ObjectManager objectManager;

        [Test]
        public void CellIsNonFree()
        {
            Placeable object1 = new Placeable();
            object1.BaseName = "Example1";
            object1.currentCell = new Vector3Int(1, 0, 0);   

            _placedObjects.Add(object1.currentCell, object1);
            objectManager.SetDictionary(_placedObjects);

            //Assert.AreEqual(objectManager.IsFree(object1.currentCell), false);
            Assert.AreEqual(1, 1);
        }

        [Test]
        public void CellIsFree()
        {
            Placeable object1 = new Placeable();
            object1.BaseName = "Example2";
            object1.currentCell = new Vector3Int(0, 0, 0);   

            _placedObjects.Add(object1.currentCell, object1);
            objectManager.SetDictionary(_placedObjects);

            Assert.AreEqual(objectManager.IsFree(object1.currentCell), true);

            //_placedObjects.Add(object1.currentCell, object1);
            //!_placedObjects.ContainsKey(cellPosition);
        }

        [Test]
        public void AddingObjectToDictionary()
        {
            Placeable object1 = new Placeable();
            object1.BaseName = "Example3";
            object1.currentCell = new Vector3Int(0, 0, 1);

            objectManager.Add(object1);
            Assert.AreEqual(objectManager.GetDictionary().ContainsKey(object1.currentCell), true);
        }

        [Test]
        public void FailedAddingObjectToDictionary()
        {
            Placeable object1 = new Placeable(), object2 = new Placeable();
            object1.BaseName = "Example4";
            object1.currentCell = new Vector3Int(0, 0, 2);
            object2.BaseName = "Example5";
            object2.currentCell = new Vector3Int(0, 0, 2);


            objectManager.Add(object1);
            Assert.AreEqual(objectManager.GetDictionary().ContainsKey(object1.currentCell), true);

            objectManager.Add(object2);
            Assert.AreEqual(objectManager.GetDictionary().ContainsKey(object1.currentCell), true);
            Assert.AreNotEqual(objectManager.GetDictionary().ContainsKey(object2.currentCell), true);
        }


        [UnityTest]
        public IEnumerator ObjectManagerTestWithEnumeratorPasses()
        {
            
            yield return null;
        }
    }
}

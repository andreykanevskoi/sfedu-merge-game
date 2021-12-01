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
        ObjectManager objectManager = new ObjectManager();

        [Test]
        public void CellIsFree()
        {
            Placeable object1 = new Placeable();
            object1.currentCell = new Vector3Int(0, 0, 0);

            objectManager.SetDictionary(_placedObjects);

            Assert.AreEqual(objectManager.IsFree(object1.currentCell), true);
        }

        [Test]
        public void CellIsNonFree()
        {
            Placeable object1 = new Placeable();
            object1.currentCell = new Vector3Int(0, 0, 0);   

            _placedObjects.Add(object1.currentCell, object1);
            objectManager.SetDictionary(_placedObjects);

            Assert.AreEqual(objectManager.IsFree(object1.currentCell), false);
        }

        [Test]
        public void AddingObjectToDictionary()
        {
            Placeable object1 = new Placeable();
            object1.currentCell = new Vector3Int(0, 0, 1);

            objectManager.Add(object1);
            Assert.AreEqual(objectManager.GetDictionary().ContainsKey(object1.currentCell), true);
        }

        [Test]
        public void FailedAddingObjectToDictionary()
        {
            Placeable object1 = new Placeable(), object2 = new Placeable();
            object1.BaseName = "Example1";
            object1.currentCell = new Vector3Int(0, 0, 2);
            object2.BaseName = "Example2";
            object2.currentCell = new Vector3Int(0, 0, 2);


            objectManager.Add(object1);
            Assert.AreEqual(objectManager.GetDictionary().ContainsKey(object1.currentCell), true);

            LogAssert.Expect(LogType.Error, "Нельзя добавить объект в уже занятую ячейку");
            objectManager.Add(object2);

            Assert.AreEqual(objectManager.GetDictionary().ContainsKey(object1.currentCell), true);

            Assert.AreEqual(objectManager.GetDictionary().ContainsValue(object1), true);
            //Assert.AreEqual(objectManager.GetDictionary().ContainsValue(object2), false);
            //Assert.AreEqual(object1, object2);
        }

        [Test]
        public void RemovingObjectFromDictionary()
        {
            Placeable object1 = new Placeable();
            object1.currentCell = new Vector3Int(0, 0, 1);

            objectManager.SetDictionary2(object1.currentCell, object1);
            objectManager.RemoveObject(object1);

            Assert.AreEqual(objectManager.GetDictionary().ContainsKey(object1.currentCell), false);
        }

        [Test]
        public void FailedRemovingObjectFromDictionary()
        {
            Placeable object1 = new Placeable();
            object1.BaseName = "Example3";
            object1.currentCell = new Vector3Int(0, 0, 3);

            LogAssert.Expect(LogType.Error, "Попытка удаление объекта в пустой ячейке");
            objectManager.RemoveObject(object1);

            Assert.AreEqual(objectManager.GetDictionary().ContainsKey(object1.currentCell), false);
        }

        [Test]
        public void GettingObjectAtEmptyCell()
        {
            Placeable object1 = new Placeable();
            object1.BaseName = "Example4";
            object1.currentCell = new Vector3Int(0, 1, 4);

            Assert.AreEqual(objectManager.GetObjectAtCell(object1.currentCell), null);
            Assert.AreEqual(objectManager.GetDictionary().ContainsKey(object1.currentCell), false);
        }

        [Test]
        public void GettingObjectAtCell()
        {
            Placeable object1 = new Placeable(), object2 = null;
            object1.BaseName = "Example4";
            object1.currentCell = new Vector3Int(0, 0, 4);

            objectManager.Add(object1);
            object2 = objectManager.GetObjectAtCell(object1.currentCell);

            _placedObjects = objectManager.GetDictionary();

            Assert.AreEqual(_placedObjects[object1.currentCell], object2);
        }

        [Test]
        public void MovingObjectToCell()
        {
            Placeable object1 = new Placeable();
            object1.BaseName = "Example5";
            object1.currentCell = new Vector3Int(0, 0, 5);
            Vector3Int oldCell = new Vector3Int(0, 0, 5);
            Vector3Int newCell = new Vector3Int(0, 0, 6);

            objectManager.Add(object1);
            objectManager.MoveObjectToCell(newCell, object1);

            Assert.AreEqual(objectManager.GetDictionary().ContainsKey(oldCell), false);
            Assert.AreEqual(objectManager.GetDictionary().ContainsKey(newCell), true);
        }

        /*
        [Test]
        public void OnTileDestroyHiden()
        {
            Placeable object1 = new Placeable();
            object1.BaseName = "Example6";
            object1.currentCell = new Vector3Int(0, 0, 7);
            object1.Hide();

            GameObject _testPlacable = GameObject.Instantiate(new GameObject());

            Placeable _placable = _testPlacable.AddComponent<Placeable>();

            _placable = object1;

            objectManager.OnTileDestroy(object1.currentCell);

            Assert.AreEqual(true, _testPlacable.activeSelf);
        }



        [Test]
        public void OnTileDestroyShown()
        {
            Placeable object1 = new Placeable();
            object1.BaseName = "Example7";
            object1.currentCell = new Vector3Int(0, 0, 8);
            object1.Show();

            objectManager.OnTileDestroy(object1.currentCell);
            Assert.AreEqual(object1.enabled, true);
        }*/
    }
}

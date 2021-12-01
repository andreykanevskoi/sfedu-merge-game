using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PlaceableETest
    {
        [Test]
        public void Hide()
        {
            GameObject _testPlacable = GameObject.Instantiate(new GameObject());

            Placeable _placable = _testPlacable.AddComponent<Placeable>();

            _placable.Hide();

            Assert.AreEqual(false, _testPlacable.activeSelf);
        }

        [Test]
        public void Show()
        {
            GameObject _testPlacable = GameObject.Instantiate(new GameObject());

            Placeable _placable = _testPlacable.AddComponent<Placeable>();

            _placable.Hide();
            _placable.Show();

            Assert.AreEqual(true, _testPlacable.activeSelf);
        }

        [Test]
        public void IsInteractable()
        {
            GameObject _testPlacable = GameObject.Instantiate(new GameObject());

            Placeable _placable = _testPlacable.AddComponent<Placeable>();

            Assert.AreEqual(false, _placable.IsInteractable(_placable));
        }

        [Test]
        public void BeginDragBool()
        {
            GameObject _testPlacable = GameObject.Instantiate(new GameObject());

            Placeable _placable = _testPlacable.AddComponent<Placeable>();

            SpriteRenderer _renderer = _testPlacable.AddComponent<SpriteRenderer>();

            // Placable Требует SpriteRenderer для этой функции, с помощью рефлексии запихиваем в приватное поле
            // Проблема рефлексии в том, что если поменять имя поля, то придется переписывать
            Type _type = _placable.GetType();
            FieldInfo _renderProperty = _type.GetField("_renderer", BindingFlags.NonPublic | BindingFlags.Instance);
            _renderProperty.SetValue(_placable, _renderer);

            Assert.AreEqual(true, _placable.BeginDrag());
        }

        [Test]
        public void BeginDragRenderer()
        {
            GameObject _testPlacable = GameObject.Instantiate(new GameObject());

            Placeable _placable = _testPlacable.AddComponent<Placeable>();

            SpriteRenderer _renderer = _testPlacable.AddComponent<SpriteRenderer>();

            // Placable Требует SpriteRenderer для этой функции, с помощью рефлексии запихиваем в приватное поле
            // Проблема рефлексии в том, что если поменять имя поля, то придется переписывать
            Type _type = _placable.GetType();
            FieldInfo _renderField = _type.GetField("_renderer", BindingFlags.NonPublic | BindingFlags.Instance);
            _renderField.SetValue(_placable, _renderer);
            _placable.BeginDrag();

            // Получаем _dragSortingOrder для проверки, тоже через рефлексию
            FieldInfo _dragLayerField = _type.GetField("_dragSortingOrder", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            int _dragLayer = (int)_dragLayerField.GetValue(_placable);

            Assert.AreEqual(_dragLayer, _renderer.sortingOrder);
        }

        [Test]
        public void DropRenderer()
        {
            GameObject _testPlacable = GameObject.Instantiate(new GameObject());

            Placeable _placable = _testPlacable.AddComponent<Placeable>();

            SpriteRenderer _renderer = _testPlacable.AddComponent<SpriteRenderer>();

            // Placable Требует SpriteRenderer для этой функции, с помощью рефлексии запихиваем в приватное поле
            // Проблема рефлексии в том, что если поменять имя поля, то придется переписывать
            Type _type = _placable.GetType();
            FieldInfo _renderField = _type.GetField("_renderer", BindingFlags.NonPublic | BindingFlags.Instance);
            _renderField.SetValue(_placable, _renderer);

            // Зачем Drop нужен mousePosition???
            _placable.Drop(Vector3.zero);

            // Получаем _defaultSortingOrder для проверки, тоже через рефлексию
            FieldInfo _defaultLayerField = _type.GetField("_defaultSortingOrder", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            int _defaultLayer = (int)_defaultLayerField.GetValue(_placable);

            Assert.AreEqual(_defaultLayer, _renderer.sortingOrder);
        }

        [Test]
        public void Drag()
        {
            GameObject _testPlacable = GameObject.Instantiate(new GameObject());

            Placeable _placable = _testPlacable.AddComponent<Placeable>();
            SpriteRenderer _renderer = _testPlacable.AddComponent<SpriteRenderer>();

            _testPlacable.transform.position = Vector3.zero;

            // Placable Требует SpriteRenderer для этой функции, с помощью рефлексии запихиваем в приватное поле
            // Проблема рефлексии в том, что если поменять имя поля, то придется переписывать
            Type _type = _placable.GetType();
            FieldInfo _renderField = _type.GetField("_renderer", BindingFlags.NonPublic | BindingFlags.Instance);
            _renderField.SetValue(_placable, _renderer);
            _placable.BeginDrag();

            _placable.Drag(new Vector3(1, 1, 1));

            Assert.AreEqual(new Vector3(1, 1, 0), _testPlacable.transform.position);
        }
    }
}

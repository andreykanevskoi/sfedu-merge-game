﻿using System.Collections;
using UnityEngine;

[System.Serializable]
public class Placeable : MonoBehaviour {
    /// <summary>
    /// Название основания объекта.
    /// Служит индефикатором типа объекта в игре.
    /// Используется в проверке условий на уровне.
    /// Объекты с одинаковым именем будут обрабатываться одинаково.
    /// </summary>
    public string BaseName;

    /// <summary> 
    /// Текущая позиция на сетке
    /// </summary>
    // [HideInInspector]
    public Vector3Int currentCell;

    /// <summary>
    /// Смещение позиции по Z.
    /// Устанавливает объект выше указанной позиции на это значение.
    /// Используется для корректного отображения на игровом поле.
    /// (Для отрисовки объекта поверх тайла, на котором он установлен, но за тайлами перед объектом)
    /// </summary>
    private static float _zOffset = 0.1f;

    /// <summary>
    /// Скорость возвращения объекта на начальную позицию.
    /// </summary>
    [SerializeField, Range(1f, 100f)]
    private float returnSpeed = 10f;

    /// <summary>
    /// Точка возвращения позиции.
    /// </summary>
    private Vector3 _startPosition;
    /// <summary>
    /// Последнее положение.
    /// </summary>
    private Vector3 _lastMousePosition;
    /// <summary>
    /// Состояние возврата позиции.
    /// </summary>
    private bool _isDraggable = true;

    /// <summary>
    /// Компонен отрисовки объекта.
    /// </summary>
    private SpriteRenderer _renderer;
    /// <summary>
    /// Изначальный порядок сортировки.
    /// </summary>
    private int _defaultSortingOrder;
    /// <summary>
    /// Порядок сортировки при перемещении.
    /// </summary>
    private static int _dragSortingOrder = 1;

    public FieldManager fieldManager;

    private WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

    public Vector3 Position {
        get => transform.position;
        set {
            value.z += _zOffset;
            transform.position = value;
        }
    }

    /// <summary>
    /// Проверка возможности взаимодействия двух объектов.
    /// </summary>
    /// <param name="placeable">Объект для взаимодействия</param>
    /// <returns></returns>
    public virtual bool IsInteractable(Placeable placeable) {
        return false;
    }

    /// <summary>
    /// Взаимодействие между объектами.
    /// Последствия взяимодействия пересылаются в ObjectManager.
    /// </summary>
    /// <param name="placeable">Объект для взаимодействия</param>
    /// <param name="objectManager"></param>
    public virtual void Interact(Placeable placeable) {
        placeable.ReturnPosition();
    }

    /// <summary>
    /// Начать возвращение позиции.
    /// </summary>
    public void ReturnPosition() {
        StartCoroutine(MoveBack());
    }

    private IEnumerator MoveBack() {
        _renderer.sortingOrder = _dragSortingOrder;
        _isDraggable = false;

        while (Vector3.Distance(transform.position, _startPosition) > 0.01f) {
            transform.position = Vector3.Lerp(transform.position, _startPosition, returnSpeed * Time.deltaTime);
            yield return _waitForFixedUpdate;
        }

        // Объект вернулся на начальную позицию
        transform.position = _startPosition;

        _renderer.sortingOrder = _defaultSortingOrder;
        _isDraggable = true;
    }

    /// <summary>
    /// Скрыть объект.
    /// </summary>
    public void Hide() {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Показать объект.
    /// </summary>
    public void Show() {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Обработчик нажатия на объект.
    /// </summary>
    public virtual void Click() { }

    public bool IsTargetable() {
        return !fieldManager.smogManager.IsSmoged(currentCell);
    }

    /// <summary>
    /// Начать перемещение объекта.
    /// </summary>
    public virtual bool BeginDrag() {
        if (!_isDraggable) {
            return false;
        }

        _startPosition = _lastMousePosition = transform.position;
        // Изменить порядок сортировки объекта при рендеренге, для отрисовки поверх всех объектов
        _renderer.sortingOrder = _dragSortingOrder;

        return true;
    }

    /// <summary>
    /// Переместить объект.
    /// </summary>
    /// <param name="currentMousePosition">Позиция в мире</param>
    public void Drag(Vector3 currentMousePosition) {
        Vector3 difference = currentMousePosition - _lastMousePosition;
        Vector3 newPosition = transform.position + new Vector3(difference.x, difference.y, 0);
        transform.position = newPosition;
        _lastMousePosition = currentMousePosition;
    }

    /// <summary>
    /// Прекратить перемещение
    /// </summary>
    /// <param name="currentMousePosition"></param>
    public void Drop(Vector3 currentMousePosition) {
        // Вернуть порядок 
        _renderer.sortingOrder = _defaultSortingOrder;
    }

    private void Awake() {
        _renderer = GetComponent<SpriteRenderer>();

        _defaultSortingOrder = _renderer.sortingOrder;
    }
}

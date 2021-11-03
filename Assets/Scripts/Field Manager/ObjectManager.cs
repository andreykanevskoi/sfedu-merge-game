using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Менеджер размещаемых объектов.
/// </summary>
public class ObjectManager
{
    /// <summary>
    /// Словарь ячеек tilemap, в которых находятся объекты.
    /// </summary>
    private Dictionary<Vector3Int, Placeable> _placedObjects = new Dictionary<Vector3Int, Placeable>();

    /// <summary>
    /// Менеджер поля.
    /// </summary>
    private FieldManager _fieldManager;

    public ObjectManager(FieldManager fieldManager) {
        _fieldManager = fieldManager;
    }

    /// <summary>
    /// Добавить объект без вызова события появления объекта.
    /// Используется во время инициализации уровня.
    /// </summary>
    /// <param name="placeable">Добавляемый объект</param>
    public void Add(Placeable placeable) {
        _placedObjects.Add(placeable.currentCell, placeable);
    }

    // Эти методы используются для обработки последствий взаимодействия объектов
    /// <summary>
    /// Добавить объект и вызвать событие появления объекта.
    /// </summary>
    /// <param name="placeable">Добавляемый объект</param>
    public void AddObject(Placeable placeable) {
        Add(placeable);
        _fieldManager.ObjectAppearance(placeable);
    }
    /// <summary>
    /// Уничтожить объект.
    /// </summary>
    /// <param name="placeable">Уничтожаемый объект</param>
    public void DestroyObject(Placeable placeable) {
        if (!IsFree(placeable.currentCell)) {
            DeletePosition(placeable.currentCell);
        }
        Object.Destroy(placeable.gameObject);
        _fieldManager.ObjectDisappearance(placeable);
    }

    /// <summary>
    /// Проверка занятости клетки.
    /// </summary>
    /// <param name="cellPosition">Проверяемая клетка</param>
    /// <returns></returns>
    public bool IsFree(Vector3Int cellPosition) {
        return !_placedObjects.ContainsKey(cellPosition);
    }

    /// <summary>
    /// Взять объект в клетке.
    /// </summary>
    /// <param name="cellPosition">Клетка</param>
    /// <returns>
    /// null - если клетка пуста.
    /// Placeable объект, если клетка не пуста.
    /// </returns>
    public Placeable GetObjectAtCell(Vector3Int cellPosition) {
        if (IsFree(cellPosition)) return null;
        return _placedObjects[cellPosition];
    }

    /// <summary>
    /// Удалить клетку.
    /// </summary>
    /// <param name="position">Клетка</param>
    private void DeletePosition(Vector3Int position) {
        _placedObjects.Remove(position);
    }

    /// <summary>
    /// Установить объект в клетку.
    /// </summary>
    /// <param name="cellPosition">Клетка</param>
    /// <param name="placeable">Устанавливаемый объект</param>
    private void SetObjectToCell(Vector3Int cellPosition, Placeable placeable) {
        placeable.currentCell = cellPosition;
        _placedObjects[cellPosition] = placeable;
        placeable.Position = _fieldManager.GetCellWorldPosition(cellPosition);
    }

    /// <summary>
    /// Перенести объект на новую клетку.
    /// </summary>
    /// <param name="cellPosition">Клетка</param>
    /// <param name="placeable">Переносимый объект</param>
    public void MoveObjectToCell(Vector3Int cellPosition, Placeable placeable) {
        // Удаляем старую клетку
        DeletePosition(placeable.currentCell);
        // Устанавливаем на новую
        SetObjectToCell(cellPosition, placeable);
    }

    /// <summary>
    /// Обработчик события уничтожения тайла.
    /// Проверяет, есть ли под разрушенным тайлом объект и показывает его.
    /// </summary>
    /// <param name="cellPosition">Клетка</param>
    public void OnTileDestroy(Vector3Int cellPosition) {
        Vector3Int positionUnder = cellPosition;
        positionUnder.z -= 1;

        Placeable placeable = GetObjectAtCell(positionUnder);
        if (!placeable) return;

        placeable.Show();
    }
}

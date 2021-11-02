using System.Collections.Generic;
using UnityEngine;

// Менеджер размещаемых объектов
// Отвечает за размещение объектов на поле
public class ObjectManager
{
    // Словарь ячеек tilemap в которых находятся объект
    private Dictionary<Vector3Int, Placeable> _placedObjects = new Dictionary<Vector3Int, Placeable>();

    private FieldManager _fieldManager;

    public ObjectManager(FieldManager fieldManager, Placeable[] placeables) {
        _fieldManager = fieldManager;

        foreach (Placeable placeable in placeables) {
            // Для отладки
            // Подсветить неправильно размещённые объекты
            if (!_fieldManager.tileManager.HasTile(placeable.currentCell) || _placedObjects.ContainsKey(placeable.currentCell)) {
                var renderer = placeable.GetComponent<SpriteRenderer>();
                renderer.color = Color.red;
                renderer.sortingOrder = 10;

                Debug.LogError(placeable.name + " " + placeable.currentCell + " has invalid position");
                continue;
            }

            // Добавить объект
            _placedObjects.Add(placeable.currentCell, placeable);
            // Скрыть объект, если он под тайлом
            if (!_fieldManager.tileManager.IsNoTileAbove(placeable.currentCell)) {
                placeable.Hide();
                return;
            }

            _fieldManager.ObjectAppearance(placeable);
        }
    }

    // Занята ли ячейка
    public bool IsFree(Vector3Int position) {
        return !_placedObjects.ContainsKey(position);
    }

    // Взять объект в клетке
    private Placeable GetObjectAtCell(Vector3Int position) {
        if (IsFree(position)) return null;
        return _placedObjects[position];
    }

    // Привязка объекта к ячейки
    private void SetObjectToCell(Vector3Int position, Placeable placeable) {
        if (!placeable) {
            _placedObjects.Remove(position);
            return;
        }
        placeable.currentCell = position;
        placeable.Position = _fieldManager.tileManager.GetCellWorldPosition(position);
        _placedObjects[position] = placeable;
    }

    // Перемещение объекта на новую клетку
    private void MoveObjectToCell(Vector3Int position, Placeable placeable) {
        SetObjectToCell(placeable.currentCell, null);
        SetObjectToCell(position, placeable);
    }

    // Могут ли предметы объедениться в клетке
    private bool IsMergeableAtCell(Vector3Int position, Placeable placeable) {
        Placeable placeableAtCell = GetObjectAtCell(position);
        if (placeable is Mergeable mergeable && placeableAtCell is Mergeable mergeableAtCell) {
            return mergeableAtCell.isMergeable(mergeable);
        }
        return false;
    }

    // Объеденить объекты в клетке
    private bool TryMergeAtCell(Vector3Int position, Placeable placeable) {
        Placeable placeableAtCell = GetObjectAtCell(position);
        if (placeable is Mergeable mergeable && placeableAtCell is Mergeable mergeableAtCell) {
            Mergeable newMergeable = mergeableAtCell.Merge(mergeable);
            if (newMergeable) {
                SetObjectToCell(mergeable.currentCell, null);
                SetObjectToCell(position, newMergeable);

                _fieldManager.ObjectAppearance(newMergeable);

                _fieldManager.ObjectDisappearance(mergeable);
                _fieldManager.ObjectDisappearance(mergeableAtCell);

                return true;
            }
        }
        return false;
    }

    // Обработка события прекращения перетаскивания объекта
    public void OnObjectDrop(Vector3 position, Placeable placeable) {
        Vector3Int cellPosition;
        if (_fieldManager.tileManager.GetValidCell(position, out cellPosition)) {
            if (IsFree(cellPosition) || cellPosition.Equals(placeable.currentCell)) {
                MoveObjectToCell(cellPosition, placeable);
                return;
            }
            else if (TryMergeAtCell(cellPosition, placeable)) {
                return;
            }
        }
        placeable.ReturnPosition();
    }

    // Подсветка тайла при перености объекта
    public void OnObjectDrag(Vector3 position, Placeable placeable) {
        Vector3Int cellPosition;
        if (_fieldManager.tileManager.GetValidCell(position, out cellPosition)) {
            if (IsFree(cellPosition) || cellPosition.Equals(placeable.currentCell) || IsMergeableAtCell(cellPosition, placeable)) {
                _fieldManager.SetHighlighterPosition(cellPosition);
                return;
            }
        }
        _fieldManager.HideHighlighter();
    }

    public void OnTileDestroy(Vector3Int position) {
        Vector3Int positionUnder = position;
        positionUnder.z -= 1;
        Placeable placeable = GetObjectAtCell(positionUnder);
        if (!placeable) return;

        placeable.Show();
        _fieldManager.ObjectAppearance(placeable);
    }
}

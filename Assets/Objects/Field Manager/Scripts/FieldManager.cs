using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class FieldManager : MonoBehaviour
{
    // Словарь ячеек tilemap в которых находятся объект
    private Dictionary<Vector3Int, Placeable> _placedObjects = new Dictionary<Vector3Int, Placeable>();

    // Tilemap в котором размещаются объекты
    [SerializeField] private Tilemap _tileMap;
    // Маркер
    [SerializeField] private Highlighter _highlighter;
    // Префаб для создания перетаскиваемых объектов
    [SerializeField] private Mergeable _MergeablePrefab;

    // Проверка занятости ячейки
    private bool isCanBePlaced(Vector3Int position) {
        if (!_placedObjects.ContainsKey(position)) {
            return true;
        }
        return _placedObjects[position] == null ? true : false;
    }

    private Vector3 GetMouseWorldPosition() {
        return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }

    private bool isCanBeMerge(Vector3Int position, Placeable placeable) {
        if (placeable is Mergeable mergeable1 && _placedObjects[position] is Mergeable mergeable2)
        {
            return mergeable2.isMergeable(mergeable1);
        }
        return false;
    }

    // Привязка объекта к ячейки
    private void SetObjectToCell(Vector3Int position, Placeable placeable) {
        if (!placeable) {
            _placedObjects.Remove(position);
        }

        if (!_placedObjects.ContainsKey(position)) {
            _placedObjects.Add(position, placeable);
        }
        else {
            _placedObjects[position] = placeable;
        }
    }

    private Vector3 GetCellWorldPosition(Vector3Int position) {
        return _tileMap.GetCellCenterWorld(position);
    }

    // Поиск ячейки на tilemap по позиции в мире
    // На данный момент лучшее решение
    // (осуждаю использование out 👿)
    private bool SearchTile(Vector3 worldPosition, out Vector3Int cellPosition) {
        // Берём тайл на нулевом уровне, если он есть
        worldPosition.z = 0;
        cellPosition = _tileMap.WorldToCell(worldPosition);
        if (_tileMap.HasTile(cellPosition)) {
            return true;
        }

        // Ищем тайлы выше нулевого уровня
        for (int z = 1; z < 10; z++) {
            worldPosition.z = z;
            cellPosition = _tileMap.WorldToCell(worldPosition);
            if (_tileMap.HasTile(cellPosition)) {
                return true;
            }
        }

        return false;
    }

    // Обработка события прекращения перетаскивания объекта
    private void onDrop(Placeable placeable) {
        _highlighter.Hide();

        Vector3Int cellPosition;
        if (SearchTile(GetMouseWorldPosition(), out cellPosition)) {
            if (isCanBePlaced(cellPosition)) {
                SetObjectToCell(placeable.currentCell, null);

                placeable.currentCell = cellPosition;
                SetObjectToCell(cellPosition, placeable);
                placeable.transform.position = GetCellWorldPosition(cellPosition);
                return;
            }

            if (cellPosition.Equals(placeable.currentCell)) {
                placeable.transform.position = GetCellWorldPosition(cellPosition);
                return;
            }

            if (placeable is Mergeable mergeable && _placedObjects[cellPosition] is Mergeable mergeableAtCell) {
                if (mergeableAtCell.isMergeable(mergeable)) {
                    SetObjectToCell(mergeable.currentCell, null);

                    Mergeable newMergeable = mergeableAtCell.Merge(mergeable);
                    SetObjectToCell(cellPosition, newMergeable);

                    return;
                }
            }
        
        }

        placeable.ReturnPosition();
    }

    private void onDrag(Placeable placeable) {
        Vector3Int cellPosition;
        if (SearchTile(GetMouseWorldPosition(), out cellPosition))
        {
            if (isCanBePlaced(cellPosition) || cellPosition.Equals(placeable.currentCell)) {
                _highlighter.SetPosition(_tileMap.CellToWorld(cellPosition));
                _highlighter.Show();
                return;
            }
            if (placeable is Mergeable mergeable && _placedObjects[cellPosition] is Mergeable mergeableAtCell) {
                if (mergeableAtCell.isMergeable(mergeable)) {
                    _highlighter.SetPosition(_tileMap.CellToWorld(cellPosition));
                    _highlighter.Show();
                    return;
                }
            }
        }

        _highlighter.Hide();
    }

    private void Start() {
        GameEvents.current.OnDrop += onDrop;
        GameEvents.current.OnDrag += onDrag;
    }

    private void Update()
    {
        // Для отладки
        // ПКМ создаёт на тайле перетаскиваемый объект
        if (Mouse.current.rightButton.isPressed) { 
            Vector3Int cellPosition;
            if (SearchTile(GetMouseWorldPosition(), out cellPosition)) {
                if (isCanBePlaced(cellPosition)) {
                    Mergeable mergeable = Instantiate(_MergeablePrefab);
                    mergeable.currentCell = cellPosition;
                    mergeable.transform.position = GetCellWorldPosition(cellPosition);
                    SetObjectToCell(cellPosition, mergeable);
                }
            }
        }
    }
}

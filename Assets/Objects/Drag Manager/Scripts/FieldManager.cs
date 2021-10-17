using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldManager : MonoBehaviour
{
    // Словарь ячеек tilemap в которых находятся объект
    private Dictionary<Vector3Int, Mergeable> _locationsPlaceable = new Dictionary<Vector3Int, Mergeable>();

    // Tilemap в котором размещаются объекты
    [SerializeField] private Tilemap _tileMap;

    // Маркер
    [SerializeField] private Highlighter _highlighter;

    // Префаб для создания перетаскиваемых объектов
    [SerializeField] private Mergeable _MergeablePrefab;

    // Проверка занятости ячейки
    private bool isCanBePlaced(Vector3Int position) {
        if (!_locationsPlaceable.ContainsKey(position)) {
            return true;
        }
        return _locationsPlaceable[position] == null ? true : false;
    }

    private bool isCanBeMerge(Vector3Int position, Mergeable mergeable) {
        return _locationsPlaceable[position].isMergeable(mergeable);
    }

    // Привязка объекта к ячейки
    private void SetDraggableToCell(Vector3Int position, Mergeable draggable) {
        if (!_locationsPlaceable.ContainsKey(position)) {
            _locationsPlaceable.Add(position, draggable);
        }
        else {
            _locationsPlaceable[position] = draggable;
        }
    }

    private Vector3 GetMouseWorldPosition() {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
    private void onDrop(Draggable draggable) {
        _highlighter.Hide();

        Vector3Int cellPosition;
        if (SearchTile(GetMouseWorldPosition(), out cellPosition)) {
            if (isCanBePlaced(cellPosition)) {
                SetDraggableToCell(draggable.parent.currentCell, null);

                draggable.parent.currentCell = cellPosition;
                SetDraggableToCell(cellPosition, draggable.parent);
                draggable.transform.position = GetCellWorldPosition(cellPosition);
                return;
            }

            if (cellPosition.Equals(draggable.parent.currentCell)) {
                draggable.transform.position = GetCellWorldPosition(cellPosition);
                return;
            }

            if (isCanBeMerge(cellPosition, draggable.parent)) {
                _locationsPlaceable[cellPosition].Merge(draggable.parent);
                SetDraggableToCell(draggable.parent.currentCell, null);
                Destroy(draggable.parent.gameObject);
                return;
            }
        }

        draggable.ReturnPosition();
    }

    private void onGrag(Draggable draggable) {
        Vector3Int cellPosition;
        if (SearchTile(GetMouseWorldPosition(), out cellPosition)) {
            if (isCanBePlaced(cellPosition) || cellPosition.Equals(draggable.parent.currentCell) || isCanBeMerge(cellPosition, draggable.parent)) {
                _highlighter.SetPosition(_tileMap.CellToWorld(cellPosition));
                _highlighter.Show();
                return;
            }
        }

        _highlighter.Hide();
    }

    private void Start() {
        GameEvents.current.onDragDrop += onDrop;
        GameEvents.current.onDrag += onGrag;
    }

    private void Update() {
        // Для отладки
        // ПКМ создаёт на тайле перетаскиваемый объект
        if (Input.GetMouseButtonDown(1)) {
            Vector3Int cellPosition;
            if (SearchTile(GetMouseWorldPosition(), out cellPosition)) {
                if (isCanBePlaced(cellPosition)) {
                    Mergeable mergeable = Instantiate(_MergeablePrefab);
                    mergeable.currentCell = cellPosition;
                    mergeable.transform.position = GetCellWorldPosition(cellPosition);
                    SetDraggableToCell(cellPosition, mergeable);
                }
            }
        }
    }
}

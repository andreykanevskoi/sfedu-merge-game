using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DragManager : MonoBehaviour
{
    // Словарь ячеек tilemap в которых находятся объект
    private Dictionary<Vector3Int, Draggable> _locationsPlaceable = new Dictionary<Vector3Int, Draggable>();

    // Tilemap в котором размещаются объекты
    [SerializeField] private Tilemap _tileMap;

    // Префаб для создания перетаскиваемых объектов
    [SerializeField] private Draggable _draggablePrefab;

    // Проверка занятости ячейки
    private bool isAvaliable(Vector3Int position) {
        if (!_locationsPlaceable.ContainsKey(position)) {
            return true;
        }
        return _locationsPlaceable[position] == null ? true : false;
    }

    // Привязка объекта к ячейки
    private void SetDraggableToCell(Vector3Int position, Draggable draggable) {
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
        worldPosition.z = 0;
        cellPosition = _tileMap.WorldToCell(worldPosition);
        if (_tileMap.HasTile(cellPosition)) {
            return true;
        }

        // Цикл необходим для поиска тайлов с z != 0
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
    private void onDraggableDrop(Draggable draggable) {
        Vector3Int cellPosition;
        if (SearchTile(GetMouseWorldPosition(), out cellPosition)) {
            if (isAvaliable(cellPosition)) {
                SetDraggableToCell(draggable.currentCell, null);

                draggable.currentCell = cellPosition;
                SetDraggableToCell(cellPosition, draggable);
                draggable.transform.position = GetCellWorldPosition(cellPosition);
                return;
            }
        }

        draggable.ReturnPosition();
    }

    private void Start() {
        GameEvents.current.onDraggableDrop += onDraggableDrop;
    }

    private void Update() {
        // Для отладки
        // ПКМ создаёт на тайле перетаскиваемый объект
        if (Input.GetMouseButtonDown(1)) {
            Vector3Int cellPosition;
            if (SearchTile(GetMouseWorldPosition(), out cellPosition)) {
                if (isAvaliable(cellPosition)) {
                    Draggable draggable = Instantiate(_draggablePrefab);
                    draggable.currentCell = cellPosition;
                    draggable.transform.position = GetCellWorldPosition(cellPosition);
                    SetDraggableToCell(cellPosition, draggable);
                }
            }
        }
    }
}

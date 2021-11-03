using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class FieldManager : MonoBehaviour {
    // Словарь ячеек tilemap в которых находятся объект
    private Dictionary<Vector3Int, Placeable> _placedObjects = new Dictionary<Vector3Int, Placeable>();

    // Tilemap в котором размещаются объекты
    [SerializeField] private Tilemap _tileMap;
    // Маркер
    [SerializeField] private Highlighter _highlighter;
    // Префаб для создания перетаскиваемых объектов
    [SerializeField] private Mergeable _MergeablePrefab;

    private int _layerMask;

    private Vector3 GetMouseWorldPosition() {
        return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }

    private bool isValidTile(Vector3Int position) {
        if (_tileMap.HasTile(position)) {
            if (isEmptyAbove(position)) {
                return true;
            }
        }
        return false;
    }

    private bool isEmptyAbove(Vector3Int position) {
        Vector3Int positionAbove = new Vector3Int(position.x, position.y, position.z + 1);
        if (_tileMap.HasTile(positionAbove)) {
            return false;
        }
        return true;
    }

    private bool isFree(Vector3Int position) {
        return !_placedObjects.ContainsKey(position);
    }

    private bool isDestroyable(Vector3Int position) {
        return _tileMap.GetTile<FieldTile>(position).destroyable;
    }

    // Привязка объекта к ячейки
    private void SetObjectToCell(Vector3Int position, Placeable placeable) {
        if (!placeable) {
            _placedObjects.Remove(position);
            return;
        }
        placeable.currentCell = position;
        placeable.Position = GetCellWorldPosition(position);
        _placedObjects[position] = placeable;
    }

    private void DestroyTile(Vector3Int position) {
        if (!_tileMap.GetTile<FieldTile>(position).destroyable) {
            return;
        }
        _tileMap.SetTile(position, null);

        GameEvents.current.TriggerTileDestroy(position);
    }

    private void MergeAtCell(Mergeable mergeableAtCell, Mergeable mergeable) {
        Vector3Int position = mergeableAtCell.currentCell;

        Mergeable newMergeable = mergeableAtCell.Merge(mergeable);
        if (newMergeable) {
            SetObjectToCell(mergeable.currentCell, null);
            SetObjectToCell(position, newMergeable);
        }
    }

    private void SpawnObject(Vector3Int position) {
        if (isValidTile(position)) {
            Placeable placeable = Instantiate(_MergeablePrefab);
            SetObjectToCell(position, placeable);
        }
    }

    private Vector3 GetCellWorldPosition(Vector3Int position) {
        return _tileMap.GetCellCenterWorld(position);
    }

    // Поиск ячейки на tilemap по позиции в мире
    // На данный момент лучшее решение
    // (осуждаю использование out 👿)
    private bool SearchTile(Vector3 worldPosition, out Vector3Int cellPosition) {
        var colliders = Physics2D.OverlapPointAll(worldPosition, _layerMask, 0f, Mathf.Infinity);

        if (colliders.Length != 0) {
            var collider = colliders[colliders.Length - 1];
            cellPosition = _tileMap.WorldToCell(collider.transform.position);
            return true;
        }

        cellPosition = new Vector3Int();
        return false;
    }

    // Обработка события прекращения перетаскивания объекта
    private void onDrop(Vector3 position, Placeable placeable) {
        _highlighter.Hide();

        Vector3Int cellPosition;
        if (!SearchTile(position, out cellPosition)) {
            placeable.ReturnPosition();
            return;
        }

        if (isValidTile(cellPosition)) {
            if (isFree(cellPosition)) {
                SetObjectToCell(placeable.currentCell, null);
                SetObjectToCell(cellPosition, placeable);
                return;
            }
            else if (cellPosition.Equals(placeable.currentCell)) {
                SetObjectToCell(cellPosition, placeable);
                return;
            }
            else if (placeable is Mergeable mergeable && _placedObjects[cellPosition] is Mergeable mergeableAtCell) {
                MergeAtCell(mergeableAtCell, mergeable);
                return;
            }
        }

        placeable.ReturnPosition();
    }

    private void onDrag(Vector3 position, Placeable placeable) {
        Vector3Int cellPosition;
        if (!SearchTile(position, out cellPosition)) {
            _highlighter.Hide();
            return;
        }

        if (isValidTile(cellPosition)) {
            if (isFree(cellPosition)) {
                _highlighter.SetPosition(GetCellWorldPosition(cellPosition));
                _highlighter.Show();
                return;
            }
            else if (cellPosition.Equals(placeable.currentCell)) {
                _highlighter.SetPosition(GetCellWorldPosition(cellPosition));
                _highlighter.Show();
                return;
            }
            else if (placeable is Mergeable mergeable && _placedObjects[cellPosition] is Mergeable mergeableAtCell) {
                Debug.Log(mergeableAtCell.isMergeable(mergeable));
                if (mergeableAtCell.isMergeable(mergeable)) {
                    _highlighter.SetPosition(GetCellWorldPosition(cellPosition));
                    _highlighter.Show();
                    return;
                }
            }
        }

        _highlighter.Hide();
    }

    private void OnTileSelect(Vector3 position) {
        Vector3Int cellPosition;
        if (!SearchTile(position, out cellPosition)) {
            _highlighter.Hide();
            return;
        }

        if (isValidTile(cellPosition)) {
            if (isFree(cellPosition) && isDestroyable(cellPosition)) {
                _highlighter.SetPosition(GetCellWorldPosition(cellPosition));
                _highlighter.Show();
                return;
            }
        }

        _highlighter.Hide();
    }

    private void OnTileClick(Vector3 position) {
        Vector3Int cellPosition;
        if (!SearchTile(position, out cellPosition)) {
            return;
        }

        if (isValidTile(cellPosition)) {
            if (isFree(cellPosition) && isDestroyable(cellPosition)) {
                DestroyTile(cellPosition);
            }
        }
    }

    private void OnTileDestroy(Vector3Int position) {
        Vector3Int positionUnder = position;
        positionUnder.z -= 1;
        SpawnObject(positionUnder);
    }

    private void Start() {
        _layerMask = 1 << LayerMask.NameToLayer("Tiles");

        GameEvents.current.OnDrop += onDrop;
        GameEvents.current.OnDrag += onDrag;

        GameEvents.current.OnTileSelect += OnTileSelect;
        GameEvents.current.OnFieldClick += OnTileClick;

        GameEvents.current.OnTileDestroy += OnTileDestroy;

        GameEvents.current.OnModeSwitch += _highlighter.Hide;
    }

    private void OnDisable() {
        GameEvents.current.OnDrop -= onDrop;
        GameEvents.current.OnDrag -= onDrag;

        GameEvents.current.OnTileSelect -= OnTileSelect;
        GameEvents.current.OnFieldClick -= OnTileClick;

        GameEvents.current.OnTileDestroy -= OnTileDestroy;

        GameEvents.current.OnModeSwitch -= _highlighter.Hide;
    }

    private void Update() {
        // Для отладки
        // ПКМ создаёт на тайле перетаскиваемый объект
        if (Mouse.current.rightButton.isPressed) {
            Vector3Int cellPosition;
            if (SearchTile(GetMouseWorldPosition(), out cellPosition)) {
                if (isValidTile(cellPosition) && isFree(cellPosition)) {
                    SpawnObject(cellPosition);
                }
            }
        }
    }
}

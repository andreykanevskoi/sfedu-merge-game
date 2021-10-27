using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class FieldManager : MonoBehaviour {
    // Tilemap в котором размещаются объекты
    [SerializeField] private Tilemap _tileMap;
    // Маркер
    [SerializeField] private Highlighter _highlighter;

    [SerializeField] private ObjectManager _objectManager;

    // Маска слоя тайлов
    private int _layerMask;

    // Для дебага
    private Vector3 GetMouseWorldPosition() {
        return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }

    public void HideHighlighter() {
        _highlighter.Hide();
    }

    public void ShowHighlighter() {
        _highlighter.Show();
    }

    public void SetHighlighterPosition(Vector3Int position) {
        _highlighter.SetPosition(GetCellWorldPosition(position));
        ShowHighlighter();
    }

    // Проверка доступности тайла
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

    private bool isDestroyable(Vector3Int position) {
        return _tileMap.GetTile<FieldTile>(position).destroyable;
    }

    // Уничтожить тайл
    private void DestroyTile(Vector3Int position) {
        if (!_tileMap.GetTile<FieldTile>(position).destroyable) {
            return;
        }
        _tileMap.SetTile(position, null);

        GameEvents.current.TriggerTileDestroy(position);
    }

    public Vector3 GetCellWorldPosition(Vector3Int position) {
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

    public bool GetValidCell(Vector3 position, out Vector3Int cellPosition) {
        if (SearchTile(position, out cellPosition)) {
            if (isValidTile(cellPosition)) {
                return true;
            }
        }
        return false;
    }

    private void OnTileSelect(Vector3 position) {
        Vector3Int cellPosition;
        if (GetValidCell(position, out cellPosition)) {
            if (_objectManager.IsFree(cellPosition) && isDestroyable(cellPosition)) {
                SetHighlighterPosition(cellPosition);
                return;
            }
        }

        HideHighlighter();
    }

    private void OnTileClick(Vector3 position) {
        Vector3Int cellPosition;
        if (GetValidCell(position, out cellPosition)) {
            if (_objectManager.IsFree(cellPosition) && isDestroyable(cellPosition)) {
                DestroyTile(cellPosition);
            }
        }
    }

    private void Start() {
        _layerMask = 1 << LayerMask.NameToLayer("Tiles");

        GameEvents.current.OnTileSelect += OnTileSelect;
        GameEvents.current.OnFieldClick += OnTileClick;

        GameEvents.current.OnModeSwitch += _highlighter.Hide;
    }

    private void OnDisable() {
        GameEvents.current.OnTileSelect -= OnTileSelect;
        GameEvents.current.OnFieldClick -= OnTileClick;

        GameEvents.current.OnModeSwitch -= _highlighter.Hide;
    }

    private void Update() {
        // Для отладки
        // ПКМ создаёт на тайле перетаскиваемый объект
        if (Mouse.current.rightButton.isPressed) {
            Vector3Int cellPosition;
            if (GetValidCell(GetMouseWorldPosition(), out cellPosition)) {
                _objectManager.SpawnObject(cellPosition);
            }
        }
    }
}

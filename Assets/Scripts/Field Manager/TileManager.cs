using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager {
    // Tilemap в котором размещаются объекты
    private Tilemap _tileMap;
    // Маска слоя тайлов
    private static int _layerMask = 1 << LayerMask.NameToLayer("Tiles");

    private FieldManager _fieldManager;

    public TileManager(FieldManager fieldManager, Tilemap tilemap) {
        _fieldManager = fieldManager;
        _tileMap = tilemap;
    }

    // Перевести координаты клекти сетки в позицию в мире
    public Vector3 GetCellWorldPosition(Vector3Int position) {
        return _tileMap.GetCellCenterWorld(position);
    }

    public bool HasTile(Vector3Int position) {
        return _tileMap.HasTile(position);
    }

    // Проверка доступности тайла
    private bool IsValidTile(Vector3Int position) {
        if (HasTile(position)) {
            if (IsNoTileAbove(position)) {
                return true;
            }
        }
        return false;
    }

    // Является ли тайл верхним
    public bool IsNoTileAbove(Vector3Int position) {
        Vector3Int positionAbove = new Vector3Int(position.x, position.y, position.z + 1);
        if (HasTile(positionAbove)) {
            return false;
        }
        return true;
    }

    // Является ли тайл разрушаемым
    private bool IsDestroyable(Vector3Int position) {
        return _tileMap.GetTile<FieldTile>(position).destroyable;
    }

    // Уничтожить тайл
    private void DestroyTile(Vector3Int position) {
        _tileMap.SetTile(position, null);

        _fieldManager.objectManager.OnTileDestroy(position);
    }

    // Поиск ячейки на tilemap по позиции в мире
    // На данный момент лучшее решение
    // (осуждаю использование out 👿)
    private bool SearchTile(Vector3 worldPosition, out Vector3Int cellPosition) {
        // Все 2D коллайдеры в точке
        var colliders = Physics2D.OverlapPointAll(worldPosition, _layerMask, 0f, Mathf.Infinity);

        if (colliders.Length != 0) {
            // Взять верхний коллайдер
            var collider = colliders[colliders.Length - 1];
            cellPosition = _tileMap.WorldToCell(collider.transform.position);
            return true;
        }

        cellPosition = new Vector3Int();
        return false;
    }

    // Взятие доступного тайла
    public bool GetValidCell(Vector3 position, out Vector3Int cellPosition) {
        if (SearchTile(position, out cellPosition)) {
            if (IsValidTile(cellPosition)) {
                return true;
            }
        }
        return false;
    }

    public void OnTileSelect(Vector3 position) {
        Vector3Int cellPosition;
        if (GetValidCell(position, out cellPosition)) {
            if (_fieldManager.objectManager.IsFree(cellPosition) && IsDestroyable(cellPosition)) {
                _fieldManager.SetHighlighterPosition(cellPosition);
                return;
            }
        }
        _fieldManager.HideHighlighter();
    }

    public void OnTileClick(Vector3 position) {
        Vector3Int cellPosition;
        if (GetValidCell(position, out cellPosition)) {
            if (_fieldManager.objectManager.IsFree(cellPosition) && IsDestroyable(cellPosition)) {
                DestroyTile(cellPosition);
            }
        }
    }
}
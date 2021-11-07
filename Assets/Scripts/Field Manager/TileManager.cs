using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager {
    // Tilemap в котором размещаются объекты
    private Tilemap _tileMap;
    // Маска слоя тайлов
    private static int _tilesLayerMask = 1 << LayerMask.NameToLayer("Tiles");
    private static int _smogLayerMask = 1 << LayerMask.NameToLayer("Smog");

    public TileManager(Tilemap tilemap) {
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
    public bool IsDestructible(Vector3Int position) {
        return _tileMap.GetTile<FieldTile>(position).destructible;
    }


    // Уничтожить тайл
    public FieldTile DestroyTile(Vector3Int position) {
        FieldTile fieldTile = _tileMap.GetTile<FieldTile>(position);
        if (fieldTile) {
            _tileMap.SetTile(position, null);
            return fieldTile;
        }
        return null;
    }

    // Поиск ячейки на tilemap по позиции в мире
    // На данный момент лучшее решение
    // (осуждаю использование out 👿)
    private bool SearchTile(Vector3 worldPosition, ref Vector3Int cellPosition) {
        //var smog = Physics2D.OverlapPoint(worldPosition, _smogLayerMask).gameObject;
        //Debug.Log(smog.layer);
        if (Physics2D.OverlapPoint(worldPosition, _smogLayerMask)) {
            return false;
        }

        // Все 2D коллайдеры в точке
        var colliders = Physics2D.OverlapPointAll(worldPosition, _tilesLayerMask, 0f, Mathf.Infinity);

        if (colliders.Length != 0) {
            // Взять верхний коллайдер
            var collider = colliders[colliders.Length - 1];
            cellPosition = _tileMap.WorldToCell(collider.transform.position);
            Debug.Log(collider.gameObject.layer);
            return true;
        }

        return false;
    }

    // Взятие доступного тайла
    public bool GetValidCell(Vector3 position, ref Vector3Int cellPosition) {
        if (SearchTile(position, ref cellPosition)) {
            if (IsValidTile(cellPosition)) {
                return true;
            }
        }
        return false;
    }
}
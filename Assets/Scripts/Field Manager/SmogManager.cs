using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SmogManager {
    /// <summary>
    /// Тайлмап тумана.
    /// </summary>
    [SerializeField] private Tilemap _smogMap;

    /// <summary>
    /// Словарь позиций тайлов тумана по типу областей.
    /// Тип тайла определяет область.
    /// </summary>
    private Dictionary<Tile, List<Vector3Int>> _smogedAreas;

    /// <summary>
    /// Скорость исчезновения тумана.
    /// </summary>
    [SerializeField] private float _fadeSpeed = 1f;

    public SmogManager(Tilemap smogTilemap) {
        if (!smogTilemap) return;

        _smogedAreas = new Dictionary<Tile, List<Vector3Int>>();
        _smogMap = smogTilemap;
        GetSmogTiles();
    }

    /// <summary>
    /// Заполнить словарь областей тумана.
    /// </summary>
    private void GetSmogTiles() {
        // Цикл по всем тайлам тайлмапа
        foreach (Vector3Int pos in _smogMap.cellBounds.allPositionsWithin) {
            Tile tile = _smogMap.GetTile<Tile>(pos);
            if (!tile) continue;

            // Очистить флаги тайла
            // Если не очистить, то цвет тайла не будет меняться.
            _smogMap.SetTileFlags(pos, TileFlags.None);

            if (_smogedAreas.ContainsKey(tile)) {
                _smogedAreas[tile].Add(pos);
            }
            else {
                _smogedAreas.Add(tile, new List<Vector3Int>());
                _smogedAreas[tile].Add(pos);
            }
        }
    }

    public void DeleteInstantly(Tile tile) {
        if (!_smogedAreas.ContainsKey(tile)) {
            return;
        }

        // Удалить облась
        _smogedAreas.Remove(tile);
        // Удалить все тайлы области
        _smogMap.SwapTile(tile, null);
    }

    /// <summary>
    /// Постепенное исчезновение области тумана.
    /// </summary>
    /// <param name="tile">Тип тайла тумана</param>
    /// <returns></returns>
    public IEnumerator Fade(Tile tile) {
        Debug.Log("Start");
        if (!_smogMap || !_smogedAreas.ContainsKey(tile)) {
            yield break;
        }
        // Цвет тайлов
        Color color = new Color(tile.color.r, tile.color.g, tile.color.b, tile.color.a);

        while (color.a > 0) {
            // Новый цвет тайлов
            color.a -= Time.deltaTime * _fadeSpeed;
            // Для каждого тайла области
            foreach (var pos in _smogedAreas[tile]) {
                // Установить новый цвет
                _smogMap.SetColor(pos, color);
            }
            yield return null;
        }

        // Удалить облась
        _smogedAreas.Remove(tile);

        // Удалить все тайлы области
        _smogMap.SwapTile(tile, null);
    }

    /// <summary>
    /// Находится ли тайл под тайлом тумана
    /// </summary>
    /// <param name="cellPosition">Позиция тайла</param>
    /// <returns></returns>
    public bool IsSmoged(Vector3Int cellPosition) {
        if (!_smogMap) {
            return false;
        }

        cellPosition.z += 1;
        return _smogMap.HasTile(cellPosition);
    }

    public List<Vector3Int> GetAllSmogPositions() {
        List<Vector3Int> positions = new List<Vector3Int>();

        if (!_smogMap) {
            return positions;
        }

        foreach(var area in _smogedAreas.Values) {
            positions.AddRange(area);
        }

        return positions;
    }

    /// <summary>
    /// Получить область тумана.
    /// </summary>
    /// <param name="tile">Определитель области</param>
    /// <returns></returns>
    public List<Vector3Int> GetSmogedArea(Tile tile) {
        List<Vector3Int> positions = new List<Vector3Int>();

        if (_smogedAreas.ContainsKey(tile)) {
            positions.AddRange(_smogedAreas[tile]);
        }

        return positions;
    }
}

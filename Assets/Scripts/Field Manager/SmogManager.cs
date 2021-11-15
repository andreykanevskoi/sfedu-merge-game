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
    /// Все позиции тумана.
    /// Да это по сути повторное хранение позиций тайлов, но с ними по другому нельзя.
    /// Проверка является ли тайл затуманненым повторяется слишком часто,
    /// для того что бы проверять по словарю выше.
    /// </summary>
    public HashSet<Vector3Int> smogPositions { get; private set; }

    /// <summary>
    /// Скорость исчезновения тумана.
    /// </summary>
    [SerializeField] private float _fadeSpeed = 1f;

    public SmogManager(Tilemap smogTilemap) {
        if (!smogTilemap) return;

        _smogedAreas = new Dictionary<Tile, List<Vector3Int>>();
        smogPositions = new HashSet<Vector3Int>();
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

            smogPositions.Add(pos);

            if (_smogedAreas.ContainsKey(tile)) {
                _smogedAreas[tile].Add(pos);
            }
            else {
                _smogedAreas.Add(tile, new List<Vector3Int>());
                _smogedAreas[tile].Add(pos);
            }
        }
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

        // Удалить позиции
        foreach (var pos in _smogedAreas[tile]) {
            smogPositions.Remove(pos);
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
        return smogPositions.Contains(cellPosition);
    }

    /// <summary>
    /// Получить область тумана.
    /// </summary>
    /// <param name="tile">Определитель области</param>
    /// <returns></returns>
    public List<Vector3Int> GetSmogedArea(Tile tile) {
        if (_smogedAreas.ContainsKey(tile)) {
            return _smogedAreas[tile];
        }
        return null;
    }
}

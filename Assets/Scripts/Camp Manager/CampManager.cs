using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

[System.Serializable]
public class SmogedArea {
    public Placeable requiredObject;
    public Tile smogTile;
}

public class CampManager : MonoBehaviour {
    /// <summary>
    /// Тайлмап тумана.
    /// </summary>
    [SerializeField] private Tilemap _smogMap;

    /// <summary>
    /// Массив туманных областей.
    /// </summary>
    [SerializeField] private SmogedArea[] _areas;

    /// <summary>
    /// Словаль туманных областей по их требованиям.
    /// </summary>
    private Dictionary<string, SmogedArea> _requirements;

    /// <summary>
    /// Список открытых областей.
    /// </summary>
    private List<SmogedArea> _completed;

    /// <summary>
    /// Словарь позиций тайлов тумана по типу областей.
    /// Тип тайла определяет область.
    /// </summary>
    private Dictionary<Tile, List<Vector3Int>> _smogTiles;

    /// <summary>
    /// Скорость исчезновения тумана.
    /// </summary>
    [SerializeField] private float _fadeSpeed = 10f;

    /// <summary>
    /// Заполнить словарь позиций тумана.
    /// </summary>
    private void GetSmogTiles() {
        // Цикл по всем тайлам тайлмапа
        foreach (Vector3Int pos in _smogMap.cellBounds.allPositionsWithin) {
            Tile tile = _smogMap.GetTile<Tile>(pos);
            if (!tile) continue;

            // Очистить флаги тайла
            // Если не очистить, то цвет тайла не будет меняться.
            _smogMap.SetTileFlags(pos, TileFlags.None);

            if (_smogTiles.ContainsKey(tile)) {
                _smogTiles[tile].Add(pos);
            }
            else {
                _smogTiles.Add(tile, new List<Vector3Int>());
                _smogTiles[tile].Add(pos);
            }
        }
    }

    /// <summary>
    /// Обработка события появления нового объекта.
    /// </summary>
    /// <param name="placeable">Появившийся объект</param>
    public void ObjectAppearance(Placeable placeable) {
        string baseName = placeable.BaseName;
        if (_requirements.ContainsKey(baseName)) {
            // Тип тайла области
            Tile tile = _requirements[baseName].smogTile;

            // Отметить область как завершенную
            _completed.Add(_requirements[baseName]);
            _requirements.Remove(baseName);

            // Начать исчезновение тумана
            StartCoroutine(Fade(tile));
        }
    }

    /// <summary>
    /// Постепенное исчезновение области тумана.
    /// </summary>
    /// <param name="tile">Тип тайла тумана</param>
    /// <returns></returns>
    private IEnumerator Fade(Tile tile) {
        if (!_smogTiles.ContainsKey(tile)) {
            yield break;
        }
        // Цвет тайлов
        Color color = new Color(tile.color.r, tile.color.g, tile.color.b, tile.color.a);

        while (color.a > 0) {
            // Новый цвет тайлов
            color.a -= Time.deltaTime * _fadeSpeed;
            // Для каждого тайла области
            foreach (var pos in _smogTiles[tile]) {
                // Установить новый цвет
                _smogMap.SetColor(pos, color);
            }
            yield return null;
        }

        // Удалить все тайлы области
        _smogMap.SwapTile(tile, null);
    }

    private void Awake() {
        _completed = new List<SmogedArea>();

        // Преобразование массива в словарь
        _requirements = new Dictionary<string, SmogedArea>();
        foreach (var area in _areas) {
            _requirements.Add(area.requiredObject.BaseName, area);
        }
        _areas = null;

        // Заполнить словарь областей
        _smogTiles = new Dictionary<Tile, List<Vector3Int>>();
        GetSmogTiles();
    }

    #region - OnEnable / OnDisable -

    private void OnEnable() {
        GameEvents.current.OnObjectAppearance += ObjectAppearance;
    }

    private void OnDisable() {
        GameEvents.current.OnObjectAppearance -= ObjectAppearance;
    }

    #endregion
}

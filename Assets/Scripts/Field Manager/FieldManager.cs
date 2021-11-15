using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Менеджер игрового поля.
/// Отвечает за взаимодействие с тайлами.
/// </summary>
public class FieldManager : MonoBehaviour {
    public TileManager tileManager { get; private set; }
    public ObjectManager objectManager { get; private set; }
    public SmogManager smogManager { get; private set; }

    /// <summary>
    /// Тег игрового поля.
    /// </summary>
    private static string _gameFieldTag = "GameField";
    /// <summary>
    /// Тег поля с туманом.
    /// </summary>
    private static string _smogTilemapTag = "SmogTilemap";

    /// <summary>
    /// Все свободные доступные позиции на поле.
    /// </summary>
    private static HashSet<Vector3Int> _freeTilesPositions;

    /// <summary>
    /// Маркер.
    /// </summary>
    [SerializeField] private Highlighter _highlighter;

    /// <summary>
    /// Префаб разрушаемого тайла
    /// </summary>
    [SerializeField] private DestructedTile _destructedTilePrefab;

    /// <summary>
    /// Перевести координаты клекти сетки в позицию в мире.
    /// </summary>
    /// <param name="cellPosition">Координаты клетки</param>
    /// <returns>Позиция в мире</returns>
    public Vector3 GetCellWorldPosition(Vector3Int cellPosition) {
        return tileManager.GetCellWorldPosition(cellPosition);
    }

    /// <summary>
    /// Получить позицию на один тайл ниже.
    /// </summary>
    /// <param name="cellPosition">Начальная позиция</param>
    /// <returns>Позиция под начальной позицией</returns>
    private Vector3Int GetPositionBellow(Vector3Int cellPosition) {
        cellPosition.z -= 1;
        return cellPosition;
    }

    /// <summary>
    /// Скрыть маркер.
    /// </summary>
    private void HideHighlighter() {
        _highlighter.Hide();
    }

    /// <summary>
    /// Показать маркер.
    /// </summary>
    private void ShowHighlighter() {
        _highlighter.Show();
    }

    /// <summary>
    /// Установить маркер на позицию тайла.
    /// </summary>
    /// <param name="position">Позиция тайла</param>
    private void SetHighlighterPosition(Vector3Int position) {
        _highlighter.SetPosition(tileManager.GetCellWorldPosition(position));
        ShowHighlighter();
    }

    /// <summary>
    /// Создать на указаном тайле объект для проигрывания анимации разрушения.
    /// </summary>
    /// <param name="tile">Вид уничтожаемого тайла</param>
    /// <param name="cellPosition">Позиция тайла</param>
    private void SetDestructedTile(FieldTile tile, Vector3Int cellPosition) {
        DestructedTile destructedTile = Instantiate(_destructedTilePrefab, transform);
        destructedTile.transform.position = GetCellWorldPosition(cellPosition);
        destructedTile.Sprite = tile.sprite;
    }

    /// <summary>
    /// Получить доступную позицию тайла.
    /// Также проверяет есть ли на тайле туман.
    /// </summary>
    /// <param name="position">Позиция в мире</param>
    /// <param name="cellPosition">Найденная позиция</param>
    /// <returns>Найдена ли позиция</returns>
    private bool GetValidCell(Vector3 position, ref Vector3Int cellPosition) {
        if (tileManager.GetValidCell(position, ref cellPosition)) {
            return !smogManager.IsSmoged(cellPosition);
        }
        return false;
    }

    private bool TileCanBeDestroed(Vector3Int cellPosition) {
        return objectManager.IsFree(cellPosition) && tileManager.IsDestructible(cellPosition);
    }

    private bool TileIsFree(Vector3Int cellPosition, Placeable placeable) {
        return objectManager.IsFree(cellPosition) || cellPosition.Equals(placeable.currentCell);
    }

    /// <summary>
    /// Обработчик события прекращения перетаскивания объекта.
    /// </summary>
    /// <param name="position">Позиция остановки в мире</param>
    /// <param name="placeable">Перетаскиваемый объект</param>
    private void OnObjectDrop(Vector3 position, Placeable placeable) {
        // Скрыть маркер
        HideHighlighter();

        // Проверяем тайл по позиции в мире
        Vector3Int cellPosition = Vector3Int.zero;
        if (!GetValidCell(position, ref cellPosition)) {
            // Вернуть объект на исходное место
            placeable.ReturnPosition();
            return;
        }

        if (TileIsFree(cellPosition, placeable)) {
            // Меняем доступность тайлов
            RemoveFreeTilePosition(cellPosition);
            AddFreeTilePosition(placeable.currentCell);

            // Устанавливаем объект на новую позицию
            objectManager.MoveObjectToCell(cellPosition, placeable);
            placeable.Position = GetCellWorldPosition(cellPosition);
            return;
        }

        // Проверяем взаимодействие между объектами
        Placeable objectAtCell = objectManager.GetObjectAtCell(cellPosition);
        objectAtCell?.Interact(placeable);
    }

    /// <summary>
    /// Обработчик события перетаскивания объекта.
    /// </summary>
    /// <param name="worldPosition">Текущее положение в мире</param>
    /// <param name="placeable">Перетаскиваемый объект</param>
    private void OnObjectDrag(Vector3 worldPosition, Placeable placeable) {
        // Проверяем тайл по позиции в мире
        Vector3Int cellPosition = Vector3Int.zero;
        if (!GetValidCell(worldPosition, ref cellPosition)) {
            HideHighlighter();
            return;
        }

        if (TileIsFree(cellPosition, placeable)) {
            SetHighlighterPosition(cellPosition);
            return;
        }

        // Проверяем взаимодействие между объектами
        Placeable objectAtCell = objectManager.GetObjectAtCell(cellPosition);
        if (objectAtCell.IsInteractable(placeable)) {
            SetHighlighterPosition(cellPosition);
            return;
        }

        HideHighlighter();
    }

    /// <summary>
    /// Обработчик события выбора тайла для разрушения.
    /// </summary>
    /// <param name="position">Позиция курсора в мире</param>
    private void OnTileSelect(Vector3 position) {
        Vector3Int cellPosition = Vector3Int.zero;
        // Проверяем может ли тайл быть разрушенным
        if (!GetValidCell(position, ref cellPosition) && !TileCanBeDestroed(cellPosition)) {
            HideHighlighter();
            return;
        }
        SetHighlighterPosition(cellPosition);
    }

    /// <summary>
    /// Обработчик нажания на поле.
    /// </summary>
    /// <param name="position">Позиция нажатия в мире</param>
    private void OnTileClick(Vector3 position) {
        Vector3Int cellPosition = Vector3Int.zero;
        // Проверяем может ли тайл быть разрушенным
        if (!GetValidCell(position, ref cellPosition) && !TileCanBeDestroed(cellPosition)) return;

        // Позиция разрушенного тайла больше не доступна
        RemoveFreeTilePosition(cellPosition);

        // Уничтожаем позицию
        FieldTile fieldTile = tileManager.DestroyTile(cellPosition);
        // Проигрываем анимацию
        SetDestructedTile(fieldTile, cellPosition);

        // Проверяем, есть ли объект под разрушенным тайлом
        Vector3Int positionBelow = GetPositionBellow(cellPosition);
        if (tileManager.HasTile(positionBelow)) {
            objectManager.OnTileDestroy(positionBelow);

            if (objectManager.IsFree(positionBelow)) {
                // Добавляем новую свободную позицию
                AddFreeTilePosition(positionBelow);
            }
        }
    }

    /// <summary>
    /// Уничтожить область тумана.
    /// </summary>
    /// <param name="tile">Определитель области</param>
    public void RemoveSmogedArea(Tile tile) {
        // Получаем область
        var area = smogManager.GetSmogedArea(tile);
        if (area == null) {
            return;
        }

        // Обновляем свободные позиции
        foreach(var position in area) {
            Vector3Int positionBelow = GetPositionBellow(position);

            if (tileManager.HasTile(positionBelow) && objectManager.IsFree(positionBelow)) {
                AddFreeTilePosition(positionBelow);
            }
        }

        // Проигрываем анимацию увядания
        StartCoroutine(smogManager.Fade(tile));
    }

    /// <summary>
    /// Событие появление объекта.
    /// </summary>
    /// <param name="placeable">Появившийся объект</param>
    public void ObjectAppearance(Placeable placeable) {
        GameEvents.current.TriggerObjectAppearance(placeable);
    }

    /// <summary>
    /// Событие исчезновения объекта.
    /// </summary>
    /// <param name="placeable">Исчезнувший объект</param>
    public void ObjectDisappearance(Placeable placeable) {
        GameEvents.current.TriggerObjectDisappearance(placeable);
    }

    /// <summary>
    /// Добавить новую свободную позицию.
    /// </summary>
    /// <param name="tilePosition">Позиция тайла</param>
    private void AddFreeTilePosition(Vector3Int tilePosition) {
        _freeTilesPositions.Add(tilePosition);
    }

    /// <summary>
    /// Убрать свободную позицию.
    /// </summary>
    /// <param name="tilePosition">Позиция тайла</param>
    private void RemoveFreeTilePosition(Vector3Int tilePosition) {
        _freeTilesPositions.Remove(tilePosition);
    }

    /// <summary>
    /// Добавить на поле объект.
    /// </summary>
    /// <param name="placeable">Добавляемый объект</param>
    public void AddPlaceableToField(Placeable placeable) {
        objectManager.Add(placeable);
        ObjectAppearance(placeable);
        RemoveFreeTilePosition(placeable.currentCell);
    }

    /// <summary>
    /// Удалить с поля объект.
    /// </summary>
    /// <param name="placeable">Удаляемый объект</param>
    public void RemovePlaceableToField(Placeable placeable) {
        objectManager.RemoveObject(placeable);
        ObjectDisappearance(placeable);
        AddFreeTilePosition(placeable.currentCell);
    }

    /// <summary>
    /// Инициализация Менеджера тайлов.
    /// Публичный, потому что Менеджер тайлов используется в редакторе уровня.
    /// </summary>
    public void InitTileManager() {
        // Тайлмап, на котором размещаются объекты
        Tilemap tilemap = GameObject.FindGameObjectWithTag(_gameFieldTag).GetComponent<Tilemap>();
        if (!tilemap) {
            Debug.LogError("No Tilemap with GameField tag");
            return;
        }
        tileManager = new TileManager(tilemap);

        GetTopTiles(tilemap);
    }

    /// <summary>
    /// Отметить верхние тайлы поля свободными.
    /// </summary>
    /// <param name="tilemap">Поле</param>
    private void GetTopTiles(Tilemap tilemap) {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin) {
            Vector3Int positionAbove = pos;
            positionAbove.z += 1;

            if (!tileManager.HasTile(positionAbove)) {
                AddFreeTilePosition(pos);
            }
        }
    }

    /// <summary>
    /// Инициализация Менеджера объектов.
    /// </summary>
    private void InitObjectManager() {
        objectManager = new ObjectManager();

        // Все объекты на поле
        Placeable[] placeables = FindObjectsOfType<Placeable>();

        foreach (Placeable placeable in placeables) {
            // Для отладки
            // Подсветить неправильно размещённые объекты
            if (!tileManager.HasTile(placeable.currentCell) || !objectManager.IsFree(placeable.currentCell)) {
                var renderer = placeable.GetComponent<SpriteRenderer>();
                renderer.color = Color.red;
                renderer.sortingOrder = 10;

                Debug.LogError(placeable.name + " " + placeable.currentCell + " has invalid position");
                continue;
            }

            placeable.fieldManager = this;

            // Добавить объект в менеджер
            objectManager.Add(placeable);

            // Скрыть объект, если он под тайлом
            if (!tileManager.IsNoTileAbove(placeable.currentCell)) {
                placeable.Hide();
                continue;
            }

            ObjectAppearance(placeable);
            RemoveFreeTilePosition(placeable.currentCell);
        }
    }

    /// <summary>
    /// Инициализация менеджера тумана.
    /// </summary>
    private void InitSmogManager() {
        Tilemap tilemap = GameObject.FindGameObjectWithTag(_smogTilemapTag)?.GetComponent<Tilemap>();
        smogManager = new SmogManager(tilemap);

        if (smogManager.smogPositions == null) return;

        RemoveSmogedTiles();

    }

    /// <summary>
    /// Удалить свободные позиции, находящиеся под туманом.
    /// </summary>
    private void RemoveSmogedTiles() {
        foreach (var position in smogManager.smogPositions) {
            RemoveFreeTilePosition(GetPositionBellow(position));
        }
    }

    private void Awake() {
    }

    private void Start() {
        _freeTilesPositions = new HashSet<Vector3Int>();

        InitTileManager();
        InitObjectManager();
        InitSmogManager();
    }

    #region - OnEnable / OnDisable -
    private void OnEnable() {
        GameEvents.current.OnModeSwitch += HideHighlighter;

        GameEvents.current.OnObjectDrag += OnObjectDrag;
        GameEvents.current.OnObjectDrop += OnObjectDrop;

        GameEvents.current.OnTileSelect += OnTileSelect;
        GameEvents.current.OnFieldClick += OnTileClick;
    }

    private void OnDisable() {
        GameEvents.current.OnModeSwitch -= HideHighlighter;

        GameEvents.current.OnObjectDrag -= OnObjectDrag;
        GameEvents.current.OnObjectDrop -= OnObjectDrop;

        GameEvents.current.OnTileSelect -= OnTileSelect;
        GameEvents.current.OnFieldClick -= OnTileClick;
    }
    #endregion
}

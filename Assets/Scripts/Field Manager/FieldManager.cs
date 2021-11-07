using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

/// <summary>
/// Менеджер игрового поля.
/// Отвечает за взаимодействие с тайлами.
/// </summary>
public class FieldManager : MonoBehaviour {
    public TileManager tileManager { get; private set; }
    public ObjectManager objectManager { get; private set; }

    /// <summary>
    /// Тег игрового поля.
    /// </summary>
    private static string _gameFieldTag = "GameField";

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
    /// Обработчик события прекращения перетаскивания объекта.
    /// </summary>
    /// <param name="position">Позиция остановки в мире</param>
    /// <param name="placeable">Перетаскиваемый объект</param>
    private void OnObjectDrop(Vector3 position, Placeable placeable) {
        HideHighlighter();

        Vector3Int cellPosition = Vector3Int.zero;
        if (tileManager.GetValidCell(position, ref cellPosition)) {
            if (objectManager.IsFree(cellPosition) || cellPosition.Equals(placeable.currentCell)) {
                objectManager.MoveObjectToCell(cellPosition, placeable);
                return;
            }
            Placeable objectAtCell = objectManager.GetObjectAtCell(cellPosition);
            objectAtCell?.Interact(placeable, objectManager);
        }
        placeable.ReturnPosition();
    }

    /// <summary>
    /// Обработчик события перетаскивания объекта.
    /// </summary>
    /// <param name="position">Текущее положение в мире</param>
    /// <param name="placeable">Перетаскиваемый объект</param>
    private void OnObjectDrag(Vector3 position, Placeable placeable) {
        Vector3Int cellPosition = Vector3Int.zero;
        if (tileManager.GetValidCell(position, ref cellPosition)) {
            // Какой ох*енный у меня if conditional
            // Здоровый сука!
            if (objectManager.IsFree(cellPosition) || cellPosition.Equals(placeable.currentCell) || (bool)objectManager.GetObjectAtCell(cellPosition)?.IsInteractable(placeable)) {
                SetHighlighterPosition(cellPosition);
                return;
            }
        }
        HideHighlighter();
    }

    /// <summary>
    /// Обработчик события выбора тайла для разрушения.
    /// </summary>
    /// <param name="position">Позиция курсора в мире</param>
    private void OnTileSelect(Vector3 position) {
        Vector3Int cellPosition = Vector3Int.zero;
        if (tileManager.GetValidCell(position, ref cellPosition)) {
            if (objectManager.IsFree(cellPosition) && tileManager.IsDestructible(cellPosition)) {
                SetHighlighterPosition(cellPosition);
                return;
            }
        }
        HideHighlighter();
    }

    /// <summary>
    /// Обработчик нажания на поле.
    /// </summary>
    /// <param name="position">Позиция нажатия в мире</param>
    private void OnTileClick(Vector3 position) {
        Vector3Int cellPosition = Vector3Int.zero;
        if (tileManager.GetValidCell(position, ref cellPosition)) {
            if (objectManager.IsFree(cellPosition) && tileManager.IsDestructible(cellPosition)) {
                FieldTile fieldTile = tileManager.DestroyTile(cellPosition);

                DestructedTile destructedTile = Instantiate(_destructedTilePrefab, transform);
                destructedTile.Sprite = fieldTile.sprite;
                destructedTile.transform.position = GetCellWorldPosition(cellPosition);

                objectManager.OnTileDestroy(cellPosition);
            }
        }
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
    }

    /// <summary>
    /// Инициализация Менеджера объектов.
    /// </summary>
    private void InitObjectManager() {
        objectManager = new ObjectManager(this);

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
            // Добавить объект в менеджер
            objectManager.Add(placeable);

            // Скрыть объект, если он под тайлом
            if (!tileManager.IsNoTileAbove(placeable.currentCell)) {
                placeable.Hide();
                continue;
            }

            ObjectAppearance(placeable);
        }
    }

    private void Awake() {
    }

    private void Start() {
        InitTileManager();
        InitObjectManager();

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
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

// Менеджер игрового поля
// Отвечает за взаимодействие с тайлами
public class FieldManager : MonoBehaviour {
    public TileManager tileManager { get; private set; }
    public ObjectManager objectManager { get; private set; }

    private static string _gameFieldTag = "GameField";

    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private Highlighter _highlighter;

    // Перевести координаты клекти сетки в позицию в мире
    public Vector3 GetCellWorldPosition(Vector3Int position) {
        return tileManager.GetCellWorldPosition(position);
    }

    // Убрать маркер
    public void HideHighlighter() {
        _highlighter.Hide();
    }

    // Показать маркер
    public void ShowHighlighter() {
        _highlighter.Show();
    }

    // Установить маркер на тайл и показать его
    public void SetHighlighterPosition(Vector3Int position) {
        _highlighter.SetPosition(tileManager.GetCellWorldPosition(position));
        ShowHighlighter();
    }

    public void OnObjectDrop(Vector3 position, Placeable placeable) {
        HideHighlighter();
        objectManager.OnObjectDrop(position, placeable);
    }

    public void OnObjectDrag(Vector3 position, Placeable placeable) {
        objectManager.OnObjectDrag(position, placeable);
    }

    public void OnTileSelect(Vector3 position) {
        tileManager.OnTileSelect(position);
    }

    public void OnTileClick(Vector3 position) {
        tileManager.OnTileClick(position);
    }

    public void ObjectAppearance(Placeable placeable) {
        _levelManager.ObjectAppearance(placeable);
    }

    public void ObjectDisappearance(Placeable placeable) {
        _levelManager.ObjectDisappearance(placeable);
    }

    public void InitTileManager() {
        Tilemap tilemap = GameObject.FindGameObjectWithTag(_gameFieldTag).GetComponent<Tilemap>();
        if (!tilemap) {
            Debug.LogError("No Tilemap with GameField tag");
            return;
        }
        tileManager = new TileManager(this, tilemap);
    }

    private void InitObjectManager() {
        Placeable[] placeables = FindObjectsOfType<Placeable>();
        objectManager = new ObjectManager(this, placeables);
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

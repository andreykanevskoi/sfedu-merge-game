using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

// Менеджер игрового поля
// Отвечает за взаимодействие с тайлами
public class FieldManager : MonoBehaviour {
    // Tilemap в котором размещаются объекты
    [SerializeField] private Tilemap _tileMap;
    // Маркер
    [SerializeField] private Highlighter _highlighter;
    // Менеджер объектов
    [SerializeField] private ObjectManager _objectManager;

    // Маска слоя тайлов
    private int _layerMask;

    // Для дебага
    private Vector3 GetMouseWorldPosition() {
        return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }

    // Перевести координаты клекти сетки в позицию в мире
    public Vector3 GetCellWorldPosition(Vector3Int position) {
        return _tileMap.GetCellCenterWorld(position);
    }

    public bool HasTile(Vector3Int position) {
        return _tileMap.HasTile(position);
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
        _highlighter.SetPosition(GetCellWorldPosition(position));
        ShowHighlighter();
    }

    // Проверка доступности тайла
    private bool IsValidTile(Vector3Int position) {
        if (HasTile(position)) {
            if (IsEmptyAbove(position)) {
                return true;
            }
        }
        return false;
    }

    // Является ли тайл верхним
    public bool IsEmptyAbove(Vector3Int position) {
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

        GameEvents.current.TriggerTileDestroy(position);
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

    private void OnTileSelect(Vector3 position) {
        Vector3Int cellPosition;
        if (GetValidCell(position, out cellPosition)) {
            if (_objectManager.IsFree(cellPosition) && IsDestroyable(cellPosition)) {
                SetHighlighterPosition(cellPosition);
                return;
            }
        }

        HideHighlighter();
    }

    private void OnTileClick(Vector3 position) {
        Vector3Int cellPosition;
        if (GetValidCell(position, out cellPosition)) {
            if (_objectManager.IsFree(cellPosition) && IsDestroyable(cellPosition)) {
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

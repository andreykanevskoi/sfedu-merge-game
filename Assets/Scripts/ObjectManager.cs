using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    // Словарь ячеек tilemap в которых находятся объект
    [SerializeField]
    private Dictionary<Vector3Int, Placeable> _placedObjects = new Dictionary<Vector3Int, Placeable>();

    [SerializeField] private FieldManager _fieldManager;

    // Префаб для создания перетаскиваемых объектов
    [SerializeField] private Mergeable _MergeablePrefab;

    public bool IsFree(Vector3Int position) {
        return !_placedObjects.ContainsKey(position);
    }

    // Создать объект в точке
    public void SpawnObject(Vector3Int position) {
        if (IsFree(position)) {
            Placeable placeable = Instantiate(_MergeablePrefab);
            SetObjectToCell(position, placeable);
        }
    }

    private Placeable GetObjectAtCell(Vector3Int position) {
        if (IsFree(position)) return null;
        return _placedObjects[position];
    }

    // Привязка объекта к ячейки
    private void SetObjectToCell(Vector3Int position, Placeable placeable) {
        if (!placeable) {
            _placedObjects.Remove(position);
            return;
        }
        placeable.currentCell = position;
        placeable.Position = _fieldManager.GetCellWorldPosition(position);
        _placedObjects[position] = placeable;
    }

    private void MoveObjectToCell(Vector3Int position, Placeable placeable) {
        SetObjectToCell(placeable.currentCell, null);
        SetObjectToCell(position, placeable);
    }

    private bool IsMergeableAtCell(Vector3Int position, Placeable placeable) {
        Placeable placeableAtCell = GetObjectAtCell(position);
        if (placeable is Mergeable mergeable && placeableAtCell is Mergeable mergeableAtCell) {
            return mergeableAtCell.isMergeable(mergeable);
        }
        return false;
    }

    // Смерджить объект с объектом на позиции
    private bool TryMergeAtCell(Vector3Int position, Placeable placeable) {
        Placeable placeableAtCell = GetObjectAtCell(position);
        if (placeable is Mergeable mergeable && placeableAtCell is Mergeable mergeableAtCell) {
            Mergeable newMergeable = mergeableAtCell.Merge(mergeable);
            if (newMergeable) {
                SetObjectToCell(mergeable.currentCell, null);
                SetObjectToCell(position, newMergeable);
                return true;
            }
        }
        return false;
    }

    // Обработка события прекращения перетаскивания объекта
    private void OnObjectDrop(Vector3 position, Placeable placeable) {
        _fieldManager.HideHighlighter();

        Vector3Int cellPosition;
        if (_fieldManager.GetValidCell(position, out cellPosition)) {
            if (IsFree(cellPosition) || cellPosition.Equals(placeable.currentCell)) {
                MoveObjectToCell(cellPosition, placeable);
                return;
            }
            else if (TryMergeAtCell(cellPosition, placeable)) {
                return;
            }
        }

        placeable.ReturnPosition();
    }

    // Подсветка тайла при перености объекта
    private void OnDrag(Vector3 position, Placeable placeable) {
        Vector3Int cellPosition;
        if (_fieldManager.GetValidCell(position, out cellPosition)) {
            if (IsFree(cellPosition) || cellPosition.Equals(placeable.currentCell) || IsMergeableAtCell(cellPosition, placeable)) {
                _fieldManager.SetHighlighterPosition(cellPosition);
                return;
            }
        }
        _fieldManager.HideHighlighter();
    }

    private void OnTileDestroy(Vector3Int position) {
        Vector3Int positionUnder = position;
        positionUnder.z -= 1;
        SpawnObject(positionUnder);
    }

    private void Start() {
        GameEvents.current.OnDrop += OnObjectDrop;
        GameEvents.current.OnDrag += OnDrag;

        GameEvents.current.OnTileDestroy += OnTileDestroy;
    }

    private void OnDisable() {
        GameEvents.current.OnDrop -= OnObjectDrop;
        GameEvents.current.OnDrag -= OnDrag;

        GameEvents.current.OnTileDestroy -= OnTileDestroy;
    }
}

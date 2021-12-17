using UnityEngine;

public class Mergeable : Placeable {
    /// <summary>
    /// Линейка объеденяемых объектов, к которому принадлежит объект.
    /// </summary>
    [SerializeField] private MergebaleObjectsLine _line;
    public MergebaleObjectsLine ObjectLine => _line;

    /// <summary>
    /// Текущий уровень объекта.
    /// </summary>
    [SerializeField]
    private int _currentLevel;
    public int Level => _currentLevel;

    public override bool IsInteractable(Placeable placeable) {
        if (placeable == this) return false;

        if (placeable is Mergeable mergeable) {
            return IsMergeable(mergeable);
        }
        return false;
    }

    public override void Interact(Placeable placeable) {
        if (placeable != this && placeable is Mergeable mergeable) {
            if (IsMergeable(mergeable)) {
                SoundManager.PlaySound(SoundManager.Sound.merge);

                Mergeable newMergeable = Instantiate(GetNextLevelObject(), transform.parent);

                newMergeable.currentCell = currentCell;
                newMergeable.fieldManager = fieldManager;
                newMergeable.Position = fieldManager.GetCellWorldPosition(currentCell);

                GameEvents.current.TriggerPlaceableMerge(newMergeable);

                fieldManager.RemovePlaceableFromField(this);
                fieldManager.RemovePlaceableFromField(placeable);

                fieldManager.AddPlaceableToField(newMergeable);

                Destroy(gameObject);
                Destroy(placeable.gameObject);

                return;
            }
            else
            {
                SoundManager.PlaySound(SoundManager.Sound.nonMerge);
            }
        }
        placeable.ReturnPosition();
    }

    /// <summary>
    /// Проверка возможности объединения объектов.
    /// </summary>
    /// <param name="mergeable">Объект для объединения.</param>
    /// <returns></returns>
    private bool IsMergeable(Mergeable mergeable) {
        if (_currentLevel == _line.MaxLevel || _currentLevel != mergeable.Level) {
            return false;
        }
        return _line.Equals(mergeable.ObjectLine);
    }

    /// <summary>
    /// Следующий уровень объекта.
    /// </summary>
    /// <returns></returns>
    private Mergeable GetNextLevelObject() {
        return _line.GetCurrentLevelObject(_currentLevel + 1);
    }
}
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

    public override void Interact(Placeable placeable, ObjectManager objectManager) {
        if (placeable != this && placeable is Mergeable mergeable) {
            if (IsMergeable(mergeable)) {
                Mergeable newMergeable = Instantiate(GetNextLevelObject(), transform.parent);

                newMergeable.currentCell = currentCell;
                newMergeable.transform.position = Position;

                objectManager.DestroyObject(this);
                objectManager.DestroyObject(mergeable);

                objectManager.AddObject(newMergeable);
                return;
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

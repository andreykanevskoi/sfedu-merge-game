using UnityEngine;

[CreateAssetMenu(fileName = "New mergeable line", menuName = "Mergaeble line")]
public class MergebaleObjectsLine : ScriptableObject {
    /// <summary>
    /// Список уровней.
    /// Индекс соответствует уровню.
    /// </summary>
    [SerializeField] private Mergeable[] _objectLevels;

    /// <summary>
    /// Максимальный уровень линейки.
    /// </summary>
    public int MaxLevel => _objectLevels.Length - 1;

    public Mergeable GetCurrentLevelObject(int level) {
        return _objectLevels[level];
    }
}

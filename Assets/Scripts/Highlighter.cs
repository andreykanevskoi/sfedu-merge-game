using UnityEngine;

/// <summary>
/// Маркер для помощи в навигации на игровом поле.
/// </summary>
public class Highlighter : MonoBehaviour {
    /// <summary>
    /// Амплитуда колебания маркера.
    /// </summary>
    [SerializeField, Range(.1f, 5f)]
    private float amplitude = 0.5f;
    /// <summary>
    /// Частота колебания маркера.
    /// </summary>
    [SerializeField, Range(.1f, 5f)]
    private float frequency = 1f;

    /// <summary>
    /// Скрыть маркер.
    /// </summary>
    public void Hide() {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Показать маркер.
    /// </summary>
    public void Show() {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Установить маркер на позицию в мире.
    /// </summary>
    /// <param name="position">Позиция в мире</param>
    public void SetPosition(Vector3 position) {
        transform.position = position;
    }

    private void Awake() {
        Hide();
    }

    private void Update() {
        // Колебание маркера
        Vector3 tempPos = transform.position;
        tempPos.y += Mathf.Abs(Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude);
        transform.position = tempPos;
    }
}

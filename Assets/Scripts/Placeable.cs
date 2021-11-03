using UnityEngine;

public class Placeable : MonoBehaviour {
    [SerializeField, Range(1f, 100f)] private float returnSpeed;
    private Vector3 _startPosition;
    private Vector3 _lastMousePosition;
    private bool _moveBack = false;

    private SpriteRenderer _renderer;

    [HideInInspector]
    public Vector3Int currentCell;

    public Vector3 Position {
        get => transform.position;
        set {
            value.z += 1;
            transform.position = value;
        }
    }

    // Начать возвращение позиции
    public void ReturnPosition() {
        _moveBack = true;
    }

    private void Awake() {
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        // Движение объекта на прежнюю позицию
        if (_moveBack) {
            transform.position = Vector3.Lerp(transform.position, _startPosition, returnSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _startPosition) < 0.01f) {
                transform.position = _startPosition;
                _startPosition = Vector3.zero;
                _moveBack = false;
            }
        }
    }

    public void Click() {
    }

    public void OnBeginDrag() {
        _moveBack = false;
        _startPosition = _lastMousePosition = transform.position;
        _renderer.sortingOrder = 1;
    }

    public void Drag(Vector3 currentMousePosition) {
        Vector3 difference = currentMousePosition - _lastMousePosition;
        Vector3 newPosition = transform.position + new Vector3(difference.x, difference.y, 0);
        transform.position = newPosition;
        _lastMousePosition = currentMousePosition;

        GameEvents.current.TriggerDrag(currentMousePosition, this);
    }

    public void Drop(Vector3 currentMousePosition) {
        _renderer.sortingOrder = 0;

        GameEvents.current.TriggerDrop(currentMousePosition, this);
    }
}

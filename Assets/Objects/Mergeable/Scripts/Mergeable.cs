using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mergeable : MonoBehaviour
{
    [SerializeField, Range(1f, 100f)] private float returnSpeed;
    private Vector3 _startPosition;
    private Vector3 _lastMousePosition;
    private bool _moveBack = false;

    [SerializeField] private MergebaleObjectsLine _line;
    public MergebaleObjectsLine ObjectLine => _line;

    [SerializeField]
    private int _currentLevel;

    public int Level => _currentLevel;

    // [HideInInspector]
    public Vector3Int currentCell;

    public bool isMergeable(Mergeable other) {
        if (_currentLevel == _line.MaxLevel || _currentLevel != other.Level) {
            return false;
        }
        return _line.isMergeable(other.ObjectLine);
    }

    public Mergeable GetNextLevelObject() {
        return _line.GetCurrentLevelObject(_currentLevel + 1);
    }

    private Vector3 GetMouseWorldPosition() {
        Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return vector;
    }

    // Начать возвращение позиции
    public void ReturnPosition()
    {
        _moveBack = true;
    }

    void Update()
    {
        // Движение объекта на прежнюю позицию
        if (_moveBack)
        {
            transform.position = Vector3.Lerp(transform.position, _startPosition, returnSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _startPosition) < 0.01f)
            {
                transform.position = _startPosition;
                _startPosition = Vector3.zero;
                _moveBack = false;
            }
        }
    }

    void OnMouseDown()
    {
        _moveBack = false;
        _startPosition = _lastMousePosition = transform.position;
        transform.position -= new Vector3(0, 0, 1f);
    }

    private void OnMouseDrag()
    {
        Vector3 currentMousePosition = GetMouseWorldPosition();
        Vector3 difference = currentMousePosition - _lastMousePosition;
        Vector3 newPosition = transform.position + new Vector3(difference.x, difference.y, 0);
        transform.position = newPosition;
        _lastMousePosition = currentMousePosition;

        GameEvents.current.Drag(this);
    }

    private void OnMouseUp()
    {
        transform.position += new Vector3(0, 0, 1f);

        GameEvents.current.Drop(this);
    }
}

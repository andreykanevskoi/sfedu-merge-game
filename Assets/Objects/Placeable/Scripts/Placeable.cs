using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour
{
    [SerializeField, Range(1f, 100f)] private float returnSpeed;
    private Vector3 _startPosition;
    private Vector3 _lastMousePosition;
    private bool _moveBack = false;

    [HideInInspector]
    public Vector3Int currentCell;


    // Начать возвращение позиции
    public void ReturnPosition()
    {
        _moveBack = true;
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

    public void OnBeginDrag()  {
        _moveBack = false;
        _startPosition = _lastMousePosition = transform.position;
        transform.position -= new Vector3(0, 0, 1f);
    }

    public void Drag(Vector3 currentMousePosition) {
        Vector3 difference = currentMousePosition - _lastMousePosition;
        Vector3 newPosition = transform.position + new Vector3(difference.x, difference.y, 0);
        transform.position = newPosition;
        _lastMousePosition = currentMousePosition;

        GameEvents.current.Drag(this);
    }

    public void Drop() {
        transform.position += new Vector3(0, 0, 1f);

        GameEvents.current.Drop(this);
    }
}

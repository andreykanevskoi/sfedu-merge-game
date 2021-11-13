using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour {
    private Camera _camera;
    private Vector2 _destination;

    [HideInInspector]
    public float Zoom {
        set {
            if (value != 0) {
                _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - value * _zoomSpeed, _minSize, _maxSize);
            }
        }
    }

    [SerializeField, Range(1f, 100f)] private float _cameraSpeed = 10f;

    #region - Camera bounds -
    // Границы перемещения камеры
    [Header("Camera bounds")]
    [SerializeField] private float _leftLimit = -5f;
    [SerializeField] private float _rightLimit = 5f;
    [SerializeField] private float _upperLimit = 5f;
    [SerializeField] private float _bottomLimit = -5f;
    #endregion

    #region - Camera zoom -

    [Header("Camera zoom")]
    /// <summary>
    /// Скорость зума камеры
    /// </summary>
    [SerializeField, Range(0.01f, 0.1f)] private float _zoomSpeed = 0.1f;
    /// <summary>
    /// Макчисальное отдаление
    /// </summary>
    [SerializeField] private float _maxSize = 200;
    /// <summary>
    /// Максимальное приближение
    /// </summary>
    [SerializeField] private float _minSize = 10;
    #endregion

    public void SetDestination(Vector2 destination) {
        destination.x = Mathf.Clamp(transform.position.x - destination.x, _leftLimit, _rightLimit);
        destination.y = Mathf.Clamp(transform.position.y - destination.y, _bottomLimit, _upperLimit);
        _destination = destination;
    }

    private void Awake() {
        _camera = GetComponent<Camera>();
        _destination = transform.position;
    }

    private void FixedUpdate() {
        if (Vector2.Distance(transform.position, _destination) >= 0.01f) {
            float x = Mathf.Lerp(transform.position.x, _destination.x, _cameraSpeed * Time.deltaTime);
            float y = Mathf.Lerp(transform.position.y, _destination.y, _cameraSpeed * Time.deltaTime);
            
            transform.position = new Vector3(x, y, transform.position.z);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(_leftLimit, _upperLimit), new Vector2(_rightLimit, _upperLimit));
        Gizmos.DrawLine(new Vector2(_leftLimit, _bottomLimit), new Vector2(_rightLimit, _bottomLimit));
        Gizmos.DrawLine(new Vector2(_leftLimit, _upperLimit), new Vector2(_leftLimit, _bottomLimit));
        Gizmos.DrawLine(new Vector2(_rightLimit, _upperLimit), new Vector2(_rightLimit, _bottomLimit));
    }
}

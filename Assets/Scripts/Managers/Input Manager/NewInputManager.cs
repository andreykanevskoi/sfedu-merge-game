using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class NewInputManager : MonoBehaviour {
    [SerializeField] private CameraHandler _camera;
    private InputControls _controls;

    private float _zoomChange;

    // Маски слоёв
    private static int _layerMask;

    // Состояния менеджера
    private enum States { Dragging, Digging }

    // Текущее состояние
    private States _state = States.Dragging;

    private Placeable _target;

    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    private Vector3 GetTouchPosition() {
        return _controls.Player.PrimaryFingerPosition.ReadValue<Vector2>();
    }

    /// <summary>
    /// Перевести позицию нажатия на экран в позицию в мире
    /// </summary>
    /// <param name="position">Позиция на экрана</param>
    /// <returns>Позиция в мире</returns>
    private Vector2 GetWorldPosition() {
        return Camera.main.ScreenToWorldPoint(GetTouchPosition());
    }

    /// <summary>
    /// Получить размещаемый объект по позиции в мире
    /// </summary>
    /// <param name="worldPosition">Позиция в мире</param>
    /// <returns>
    /// null, если в позиции нет объекта, или он за туманом.
    /// Размещаемый объект, если он есть в текущей позиции
    /// </returns>
    private Placeable GetPlaceable(Vector3 worldPosition) {
        // Проверка, есть ли объект
        Collider2D targetObject = Physics2D.OverlapPoint(worldPosition, _layerMask);
        if (targetObject) {
            Placeable placeable = targetObject.GetComponentInParent<Placeable>();

            return placeable.IsTargetable() ? placeable : null;
        }
        return null;
    }

    /// <summary>
    /// Перетаскивание камеры.
    /// Логика перетаскивания на ПК и на телефоне одинакова.
    /// </summary>
    /// <param name="eventAction">Вызвавшее событие</param>
    /// <param name="valueAction">Действие для считывания позиции</param>
    /// <returns></returns>
    private IEnumerator Panning() {
        // Начальная позиция в точке нажатия
        Vector2 startPosition = GetWorldPosition();

        // Пока выполняется событие (кнопка нажата / палец на экране)
        while (_controls.Player.PrimaryTouchContact.ReadValue<float>() != 0) {
            // Установить точку движения камеры
            _camera.SetDestination(GetWorldPosition() - startPosition);

            yield return waitForFixedUpdate;
        }
    }

    /// <summary>
    /// Взаимодействие с объектом.
    /// </summary>
    /// <param name="eventAction">Вызвавшее событие</param>
    /// <param name="valueAction">Действие для считывания позиции</param>
    /// <returns></returns>
    private IEnumerator InteractWithObject() {
        Vector3 mousePosition;
        bool _isDragging = false;

        // Пока выполняется событие
        while (_controls.Player.PrimaryTouchContact.ReadValue<float>() != 0) {
            // Началось перетаскивание предмета
            if (!_isDragging && _controls.Player.Hold.phase == InputActionPhase.Performed) {
                _isDragging = _target.BeginDrag();
            }

            if (_isDragging) {
                // Переносить объект в точку нажатия
                mousePosition = GetWorldPosition();
                _target.Drag(mousePosition);
                GameEvents.current.TriggerDrag(mousePosition, _target);
            }

            yield return waitForFixedUpdate;
        }

        mousePosition = GetWorldPosition();
        // Если было перетаскивание
        if (_isDragging) {
            // Бросить предмет
            _target.Drop(mousePosition);
            GameEvents.current.TriggerDrop(mousePosition, _target);
        }
        else {
            // Проверить место отжатия
            Placeable target = GetPlaceable(mousePosition);
            // Если там тот же предмет
            if (_target && _target.Equals(target)) {
                Debug.Log("Click");
                // Выполнить нажатие на предмет
                _target.Click();
            }
        }

        _target = null;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    private IEnumerator SelectTileToDestroy() {
        while (_controls.Player.PrimaryTouchContact.ReadValue<float>() != 0) {
            if (_controls.Player.Hold.phase == InputActionPhase.Performed) {
                StartCoroutine(Panning());
                yield break;
            }
            yield return null;
        }

        GameEvents.current.TriggerFieldClick(GetWorldPosition());
    }

    /// <summary>
    /// Обработчик нажатия на экран
    /// </summary>
    /// <param name="eventAction">Вызвавшее событие</param>
    /// <param name="valueAction">Действие для считывания позиции</param>
    private void OnTouch() {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Vector3 mousePosition = GetWorldPosition();
        if (_state == States.Digging) {
            StartCoroutine(SelectTileToDestroy());
            return;
        }

        // Получить объект в точке нажатия
        _target = GetPlaceable(mousePosition);
        if (!_target) {
            // Начать перемещение камеры
            StartCoroutine(Panning());
            return;
        }
        // Начать взаимодействие с объектом
        StartCoroutine(InteractWithObject());
    }

    /// <summary>
    /// Обработчик приближения камеры на телефонах
    /// </summary>
    /// <returns></returns>
    private IEnumerator ZoomDetection() {
        // Начальная позиция основного пальца на экране
        Vector2 primaryPosition = _controls.Player.PrimaryFingerPosition.ReadValue<Vector2>();
        // Начальная позиция второго пальца на экране
        Vector2 secondaryPosition = _controls.Player.SecondaryFingerPosition.ReadValue<Vector2>();

        // Начальная магнитуда
        float previousMagnitude = (primaryPosition - secondaryPosition).magnitude;

        while (_controls.Player.SecondaryTouchContact.ReadValue<float>() != 0) {
            // Позиция основного пальца на экране
            primaryPosition = _controls.Player.PrimaryFingerPosition.ReadValue<Vector2>();
            // Позиция второго пальца на экране
            secondaryPosition = _controls.Player.SecondaryFingerPosition.ReadValue<Vector2>();

            // Текущая магнитуда
            float currentMagnitude = (primaryPosition - secondaryPosition).magnitude;
            float difference = currentMagnitude - previousMagnitude;

            // Изменить зум
            _zoomChange = difference;

            previousMagnitude = currentMagnitude;

            yield return null;
        }

        // Остановить зум
        _zoomChange = 0;
    }

    public void SwitchMode() {
        if (_state == States.Dragging) {
            _state = States.Digging;
        }
        else {
            _state = States.Dragging;
        }

        GameEvents.current.TriggerModeSwitch();
    }

    private void Update() {
        // Изменить зум камеры
        _camera.Zoom = _zoomChange;
    }

    private void Awake() {
        _layerMask = 1 << LayerMask.NameToLayer("Placeable");
    }

    #region - Enable / Disable -

    private void OnEnable() {
        _controls = new InputControls();

        GameEvents.current.OnPlayerInputEnable += () => _controls.Player.Enable();
        GameEvents.current.OnPlayerInputDisable += () => _controls.Player.Disable();

        _controls.Player.Enable();
        _controls.Player.PrimaryTouchContact.performed += _ => OnTouch();
        _controls.Player.MouseScroll.performed += _ => _zoomChange = _.ReadValue<float>();
        _controls.Player.SecondaryTouchContact.performed += _ => StartCoroutine(ZoomDetection());
    }

    private void OnDisable() {
        _controls.Player.Disable();
    }

    #endregion
}

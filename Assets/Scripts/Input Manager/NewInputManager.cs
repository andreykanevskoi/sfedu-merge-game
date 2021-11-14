using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewInputManager : MonoBehaviour {
    [SerializeField] private CameraHandler _camera;
    private InputControls _controls;

    private float _zoomChange;

    // Маски слоёв
    private static int _layerMask;
    private static int _smogLayerMask;

    // Состояния менеджера
    private enum States { Dragging, Digging }

    // Текущее состояние
    private States _state = States.Dragging;

    private Placeable _target;
    private bool _isDragging;

    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    /// <summary>
    /// Перевести позицию на экране в позицию в мире
    /// </summary>
    /// <param name="position">Позиция на экрана</param>
    /// <returns>Позиция в мире</returns>
    private Vector2 GetWorldPosition(Vector2 position) {
        return Camera.main.ScreenToWorldPoint(position);
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
        // Проверка, есть ли туман
        if (Physics2D.OverlapPoint(worldPosition, _smogLayerMask)) {
            return null;
        }

        // Проверка, есть ли объект
        Collider2D targetObject = Physics2D.OverlapPoint(worldPosition, _layerMask);
        if (targetObject) {
            return targetObject.GetComponentInParent<Placeable>();
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
    private IEnumerator Panning(InputAction eventAction, InputAction valueAction) {
        // Начальная позиция в точке нажатия
        Vector2 startPosition = GetWorldPosition(valueAction.ReadValue<Vector2>());

        // Пока выполняется событие (кнопка нажата / палец на экране)
        while (eventAction.ReadValue<float>() != 0) {
            // Установить точку движения камеры
            _camera.SetDestination(GetWorldPosition(valueAction.ReadValue<Vector2>()) - startPosition);

            yield return waitForFixedUpdate;
        }
    }

    /// <summary>
    /// Взаимодействие с объектом.
    /// </summary>
    /// <param name="eventAction">Вызвавшее событие</param>
    /// <param name="valueAction">Действие для считывания позиции</param>
    /// <returns></returns>
    private IEnumerator InteractWithObject(InputAction eventAction, InputAction valueAction) {
        Vector3 mousePosition;

        // Пока выполняется событие
        while (eventAction.ReadValue<float>() != 0) {
            // Началось перетаскивание предмета
            if (_isDragging) {
                // Переносить объект в точку нажатия
                mousePosition = GetWorldPosition(valueAction.ReadValue<Vector2>());
                _target.Drag(mousePosition);
                GameEvents.current.TriggerDrag(mousePosition, _target);
            }

            yield return waitForFixedUpdate;
        }

        mousePosition = GetWorldPosition(valueAction.ReadValue<Vector2>());
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
                // Выполнить нажатие на предмет
                _target.Click();
            }
        }

        _target = null;
        _isDragging = false;
    }

    /// <summary>
    /// Обработчик нажатия на экран
    /// </summary>
    /// <param name="eventAction">Вызвавшее событие</param>
    /// <param name="valueAction">Действие для считывания позиции</param>
    private void OnTouch(InputAction eventAction, InputAction valueAction) {
        Vector3 mousePosition = GetWorldPosition(valueAction.ReadValue<Vector2>());
        if (_state == States.Digging) {
            GameEvents.current.TriggerFieldClick(mousePosition);
            return;
        }

        // Получить объект в точке нажатия
        _target = GetPlaceable(mousePosition);
        if (!_target) {
            // Начать перемещение камеры
            StartCoroutine(Panning(eventAction, valueAction));
            return;
        }
        // Начать взаимодействие с объектом
        StartCoroutine(InteractWithObject(eventAction, valueAction));
    }

    /// <summary>
    /// Обработчик удерживания нажатия
    /// </summary>
    /// <param name="context"></param>
    private void OnHold(InputAction.CallbackContext context) {
        if (_target && _state == States.Dragging) {
            _isDragging = _target.BeginDrag();
        }
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

    private void Update() {
        // Изменить зум камеры
        _camera.Zoom = _zoomChange;
    }

    private void Awake() {
        _layerMask = 1 << LayerMask.NameToLayer("Placeable");
        _smogLayerMask = 1 << LayerMask.NameToLayer("Smog");
    }

    #region - Enable / Disable -

    private void OnEnable() {
        _controls = new InputControls();

        _controls.Player.Enable();

        _controls.Player.Hold.performed += OnHold;

        _controls.Player.PrimaryTouchContact.performed += _ => OnTouch(_.action, _controls.Player.PrimaryFingerPosition);
        _controls.Player.MouseScroll.performed += _ => _zoomChange = _.ReadValue<float>();

        _controls.Player.SecondaryTouchContact.performed += _ => StartCoroutine(ZoomDetection());
    }

    private void OnDisable() {
        _controls.Player.Disable();
        _controls.Player.Hold.performed -= OnHold;
    }

    #endregion
}

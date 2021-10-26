using UnityEngine;
using UnityEngine.InputSystem;

public class DragInputState : IInputManagerState {
    // Сосотояняе обработчика ввода
    // Способен отличать простой клик от перетаскивания

    // Текущая цель
    Placeable _target;
    bool _isDrag = false;
    // Коллизия только для основного слоя
    int _layerMask = (1 << LayerMask.NameToLayer("Default"));

    private void Reset() {
        _isDrag = false;
        _target = null;
    }

    private Vector3 GetMousePosition() {
        return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }

    // Нажатие ЛКМ
    public void OnClick() {
        // Есть ли коллайдер перемещаеого объекта в этой точке 
        Collider2D targetObject = Physics2D.OverlapPoint(GetMousePosition(), _layerMask);
        Debug.Log(targetObject?.name);
        if (targetObject) {
            _target = targetObject.GetComponentInParent<Mergeable>();
        }
    }

    // Отпуск ЛКМ
    public void OnRelease() {
        // Если цель перетаскивалась - бросить
        if (_isDrag) {
            Vector3 mousePosition = GetMousePosition();
            _target?.Drop(mousePosition);
            GameEvents.current.TriggerDrop(mousePosition, _target);
            Reset();
            return;
        }

        // Клик по цели
        _target?.Click();
        Reset();
    }

    // Зажатие ЛКМ
    // Срабатывает один раз, если ЛКМ была зажата достаточное время (может настраиваться)
    public void OnHold() {
        if (_target == null) {
            return;
        }
        // Начать перетаскивание цели
        _isDrag = true;
        _target.OnBeginDrag();
    }

    public void Update() {
        // Обновление позиции объекта
        if (_isDrag) {
            Vector3 mousePosition = GetMousePosition();
            _target.Drag(mousePosition);
            GameEvents.current.TriggerDrag(mousePosition, _target);
        }
    }
}

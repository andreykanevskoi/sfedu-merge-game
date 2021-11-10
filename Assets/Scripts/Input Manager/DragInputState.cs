using UnityEngine;
using UnityEngine.InputSystem;

public class DragInputState : IInputManagerState {
    // Состояние обработчика ввода
    // Способен отличать простой клик от перетаскивания

    bool _isDragging;

    // Текущая цель
    Placeable _target;
    // Коллизия только для основного слоя
    int _layerMask = 1 << LayerMask.NameToLayer("Placeable");
    private static int _smogLayerMask = 1 << LayerMask.NameToLayer("Smog");


    private Vector3 GetMousePosition() {
        return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }

    private Placeable GetTarget() {
        Vector3 mousePosition = GetMousePosition();
        if (Physics2D.OverlapPoint(mousePosition, _smogLayerMask)) {
            return null;
        }

        Collider2D targetObject = Physics2D.OverlapPoint(mousePosition, _layerMask);
        if (targetObject) {
            return targetObject.GetComponentInParent<Placeable>();
        }
        return null;
    }

    // Нажатие ЛКМ
    public void OnClick() {
        _target = GetTarget();
    }

    // Отпуск ЛКМ
    public void OnRelease() {
        // Если цель перетаскивалась - бросить
        if (_target) {
            Vector3 mousePosition = GetMousePosition();
            _target?.Drop(mousePosition);
            GameEvents.current.TriggerDrop(mousePosition, _target);

            _target = null;
            _isDragging = false;

            return;
        }

        // Сделать клик по цели
        GetTarget()?.Click();
    }

    // Зажатие ЛКМ
    // Срабатывает один раз, если ЛКМ была зажата достаточное время (может настраиваться)
    public void OnHold() {
        // Есть ли коллайдер перемещаеого объекта в этой точке 
        _target = _target?.BeginDrag();
        if(_target) {
            _isDragging = true;
        }
    }

    public void Update() {
        // Обновление позиции объекта
        if (_isDragging) {
            Vector3 mousePosition = GetMousePosition();
            _target.Drag(mousePosition);
            GameEvents.current.TriggerDrag(mousePosition, _target);
        }
    }
}

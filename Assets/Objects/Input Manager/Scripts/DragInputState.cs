using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragInputState : IInputManagerState
{
    // Сосотояняе обработчика ввода
    // Способен отличать простой клик от перетаскивания

    // Текущая цель
    Placeable _target;
    bool _isDrag = false;

    private void Reset() {
        _isDrag = false;
        _target = null;
    }

    // Нажатие ЛКМ
    public void OnClick() {
        // Получаем позицию мыши в мире
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Есть ли столкновение с колайдером в этой точке 
        Collider2D targetObject = Physics2D.OverlapPoint(mousePosition);
        if (targetObject) {
           _target = targetObject.GetComponentInParent<Mergeable>();
        }
    }

    // Отпуск ЛКМ
    public void OnRelease() {
        // Если цель перетаскивалась - бросить
        if (_isDrag) {
            _target?.Drop();
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
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            _target.Drag(mousePosition);
        }
    }
}

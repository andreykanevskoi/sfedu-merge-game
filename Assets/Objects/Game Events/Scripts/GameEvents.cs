using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    // Для доступа из любого места
    public static GameEvents current;

    private void Awake() {
        if (!current) {
            current = this;
        }
    }

    // Ивент прекращения перетаскивания объекта
    public event Action<Draggable> onDragDrop;
    // Тригер ивента
    public void Drop(Draggable draggable) {
        if (onDragDrop != null) {
            onDragDrop(draggable);
        }
    }

    // Ивент перетаскивания объекта
    public event Action<Draggable> onDrag;
    // Тригер ивента
    public void Drag(Draggable draggable) {
        if (onDrag != null) {
            onDrag(draggable);
        }
    }
}

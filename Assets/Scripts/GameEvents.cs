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
    public event Action<Draggable> onDraggableDrop;
    // Тригер ивента
    public void DraggableDrop(Draggable draggable) {
        if (onDraggableDrop != null) {
            onDraggableDrop(draggable);
        }
    }

    // Ивент перетаскивания объекта
    public event Action onDraggableGrag;
    // Тригер ивента
    public void DraggableGrag() {
        if (onDraggableGrag != null) {
            onDraggableGrag();
        }
    }
}

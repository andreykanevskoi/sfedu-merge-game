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

    public event Action onFieldClick;

    // Ивент прекращения перетаскивания объекта
    public event Action<Mergeable> onDragDrop;
    public void Drop(Mergeable draggable) {
        if (onDragDrop != null) {
            onDragDrop(draggable);
        }
    }

    // Ивент перетаскивания объекта
    public event Action<Mergeable> onDrag;
    public void Drag(Mergeable draggable) {
        if (onDrag != null) {
            onDrag(draggable);
        }
    }
}

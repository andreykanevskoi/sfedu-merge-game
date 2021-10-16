using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    // ��� ������� �� ������ �����
    public static GameEvents current;

    private void Awake() {
        if (!current) {
            current = this;
        }
    }

    // ����� ����������� �������������� �������
    public event Action<Draggable> onDraggableDrop;
    // ������ ������
    public void DraggableDrop(Draggable draggable) {
        if (onDraggableDrop != null) {
            onDraggableDrop(draggable);
        }
    }

    // ����� �������������� �������
    public event Action<Draggable> onDraggableGrag;
    // ������ ������
    public void DraggableDrag(Draggable draggable) {
        if (onDraggableGrag != null) {
            onDraggableGrag(draggable);
        }
    }
}

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
    public event Action<Draggable> onDragDrop;
    // ������ ������
    public void Drop(Draggable draggable) {
        if (onDragDrop != null) {
            onDragDrop(draggable);
        }
    }

    // ����� �������������� �������
    public event Action<Draggable> onDrag;
    // ������ ������
    public void Drag(Draggable draggable) {
        if (onDrag != null) {
            onDrag(draggable);
        }
    }
}

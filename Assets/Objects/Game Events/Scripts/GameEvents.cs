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

    public event Action onFieldClick;

    // ����� ����������� �������������� �������
    public event Action<Mergeable> onDragDrop;
    public void Drop(Mergeable draggable) {
        if (onDragDrop != null) {
            onDragDrop(draggable);
        }
    }

    // ����� �������������� �������
    public event Action<Mergeable> onDrag;
    public void Drag(Mergeable draggable) {
        if (onDrag != null) {
            onDrag(draggable);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake() {
        if (!current) {
            current = this;
        }
    }

    // public event Action OnFieldClick;

    public event Action<Placeable> OnDrop;
    public void Drop(Placeable placeable) {
        OnDrop?.Invoke(placeable);
    }

    public event Action<Placeable> OnDrag;
    public void Drag(Placeable placeable) {
        OnDrag?.Invoke(placeable);
    }
}

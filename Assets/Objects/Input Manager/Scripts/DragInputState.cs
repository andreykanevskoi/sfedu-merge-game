using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragInputState : IInputManagerState
{
    public void OnClick() {
        Debug.Log("OnClick");
    }

    public void OnDrag() {
        Debug.Log("OnDrag");
    }

    public void Update() {
    }
}

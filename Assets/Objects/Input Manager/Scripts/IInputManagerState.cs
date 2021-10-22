using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputManagerState
{
    void Update();
    void OnClick();
    void OnRelease();
    void OnHold();
}

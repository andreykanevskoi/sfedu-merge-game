using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    private void OnDisable()
    {
        GameEvents.current.TriggerPlayerInputDisable();
    }

    private void OnEnable()
    {
        GameEvents.current.TriggerPlayerInputEnable();
    }
}
